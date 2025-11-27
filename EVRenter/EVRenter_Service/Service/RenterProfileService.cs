using EVRenter_Data.Entities;
using EVRenter_Repository.UnitOfWork;
using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using Microsoft.EntityFrameworkCore;
using EVRenter_CM.Enums;
using EVRenter_Service.IService;

namespace EVRenter_Service.Service
{
    public class RenterProfileService : IRenterProfileService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RenterProfileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        
        public async Task<UploadRPResponseModel> CreateOrUpdateProfileAsync(UploadRPRequestModel request)
        {
            
            var user = await _unitOfWork.Repository<User>()
                .AsQueryable()
                .FirstOrDefaultAsync(u => u.Id == request.RenterId && !u.IsDelete);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            if (user.RoleID != RoleType.Renter)
                throw new UnauthorizedAccessException("Only renter can create profile.");

            
            var pfRepo = _unitOfWork.Repository<RenterProfile>();

            var existingPF = await pfRepo.AsQueryable()
                .Where(p => p.UserID == user.Id && !p.IsDelete)
                .Include(p => p.IDImages).ThenInclude(i => i.Image)
                .Include(p => p.DriverLicenseImages).ThenInclude(i => i.Image)
                .FirstOrDefaultAsync();


            
            if (user.IsVerified == 1)
            {
                var newPF = await CreateNewPF(request, user.Id);
                user.IsVerified = 2;

                await _unitOfWork.SaveChangesAsync();
                return await BuildPFResponse(newPF, user.IsVerified);
            }


            
            if (user.IsVerified == 2 && existingPF != null)
            {
                await UpdateExistingPF(existingPF, request);
                await _unitOfWork.SaveChangesAsync();

                return await BuildPFResponse(existingPF, user.IsVerified);
            }


            
            if (user.IsVerified == 3 && existingPF != null)
            {
                existingPF.IsDelete = true;

                var newPF = await CreateNewPF(request, user.Id);
                user.IsVerified = 2;

                await _unitOfWork.SaveChangesAsync();
                return await BuildPFResponse(newPF, user.IsVerified);
            }

            throw new Exception("Unexpected profile state.");
        }


        
        private async Task<RenterProfile> CreateNewPF(UploadRPRequestModel req, int userId)
        {
            var profile = new RenterProfile
            {
                UserID = userId,
                IDNumber = req.IDNumber,
                DriverLicenseNo = req.DriverLicenseNo,
                IsDelete = false
            };

            await _unitOfWork.Repository<RenterProfile>().AddAsync(profile);
            await _unitOfWork.SaveChangesAsync();


            
            await AddImage(profile.Id, userId, req.IDCardFrontImage, 1, true);
            await AddImage(profile.Id, userId, req.IDCardBackImage, 2, true);

            await AddImage(profile.Id, userId, req.DriverLicenseFrontImage, 1, false);
            await AddImage(profile.Id, userId, req.DriverLicenseBackImage, 2, false);

            return profile;
        }



        private async Task UpdateExistingPF(RenterProfile pf, UploadRPRequestModel req)
        {
            pf.IDNumber = req.IDNumber;
            pf.DriverLicenseNo = req.DriverLicenseNo;

            var idImageRepo = _unitOfWork.Repository<IDImage>();
            var dlImageRepo = _unitOfWork.Repository<DriverLicenseImage>();
            var imageRepo = _unitOfWork.Repository<Image>();

            
            var idImgs = pf.IDImages.ToList();
            var dlImgs = pf.DriverLicenseImages.ToList();

            
            idImageRepo.DeleteRange(idImgs.AsQueryable());
            dlImageRepo.DeleteRange(dlImgs.AsQueryable());

            
            foreach (var img in idImgs)
            {
                if (img.Image != null)
                    imageRepo.Delete(img.Image);
            }

            foreach (var img in dlImgs)
            {
                if (img.Image != null)
                    imageRepo.Delete(img.Image);
            }

            await _unitOfWork.SaveChangesAsync();

            
            await AddImage(pf.Id, pf.UserID, req.IDCardFrontImage, 1, true);
            await AddImage(pf.Id, pf.UserID, req.IDCardBackImage, 2, true);
            await AddImage(pf.Id, pf.UserID, req.DriverLicenseFrontImage, 1, false);
            await AddImage(pf.Id, pf.UserID, req.DriverLicenseBackImage, 2, false);
        }



        
        private async Task AddImage(int profileId, int userId, byte[] data, int type, bool isIDCard)
        {
            var img = new Image
            {
                Base64Image = data,
                ContentType = "image/jpeg",
                IsDelete = false
            };

            await _unitOfWork.Repository<Image>().AddAsync(img);
            await _unitOfWork.SaveChangesAsync();

            if (isIDCard)
            {
                await _unitOfWork.Repository<IDImage>().AddAsync(new IDImage
                {
                    ImageID = img.Id,
                    RenterID = userId,
                    Type = type,
                    ProfileID = profileId
                });
            }
            else
            {
                await _unitOfWork.Repository<DriverLicenseImage>().AddAsync(new DriverLicenseImage
                {
                    ImageID = img.Id,
                    RenterID = userId,
                    Type = type,
                    ProfileID = profileId
                });
            }


        }


        
        private async Task<UploadRPResponseModel> BuildPFResponse(RenterProfile pf, int status)
        {
            return new UploadRPResponseModel
            {
                RenterId = pf.UserID,
                IDNumber = pf.IDNumber,
                DriverLicenseNo = pf.DriverLicenseNo,

                IDCardFrontImage = pf.IDImages.FirstOrDefault(i => i.Type == 1)?.Image?.Base64Image,
                IDCardBackImage = pf.IDImages.FirstOrDefault(i => i.Type == 2)?.Image?.Base64Image,
                DriverLicenseFrontImage = pf.DriverLicenseImages.FirstOrDefault(i => i.Type == 1)?.Image?.Base64Image,
                DriverLicenseBackImage = pf.DriverLicenseImages.FirstOrDefault(i => i.Type == 2)?.Image?.Base64Image,

                VerificationStatus = status,
                Message = status switch
                {
                    1 => "No profile found.",
                    2 => "Profile submitted and pending approval.",
                    3 => "Profile approved.",
                    _ => "Profile processed."
                }
            };
        }

        public async Task<bool> ApproveProfileAsync(int renterId)
        {
            var pfRepo = _unitOfWork.Repository<RenterProfile>();
            var userRepo = _unitOfWork.Repository<User>();
            var idImgRepo = _unitOfWork.Repository<IDImage>();
            var dlImgRepo = _unitOfWork.Repository<DriverLicenseImage>();
            var imageRepo = _unitOfWork.Repository<Image>();

            
            var newPF = await pfRepo.AsQueryable()
                .Where(p => p.UserID == renterId && !p.IsDelete)
                .Include(p => p.IDImages).ThenInclude(i => i.Image)
                .Include(p => p.DriverLicenseImages).ThenInclude(i => i.Image)
                .FirstOrDefaultAsync();

            if (newPF == null)
                throw new KeyNotFoundException("Profile not found.");

            
            var oldPF = await pfRepo.AsQueryable()
                .Where(p => p.UserID == newPF.UserID && p.IsDelete)
                .Include(p => p.IDImages).ThenInclude(i => i.Image)
                .Include(p => p.DriverLicenseImages).ThenInclude(i => i.Image)
                .FirstOrDefaultAsync();

            
            if (oldPF != null)
            {
                foreach (var img in oldPF.IDImages)
                    imageRepo.Delete(img.Image);

                foreach (var img in oldPF.DriverLicenseImages)
                    imageRepo.Delete(img.Image);

                idImgRepo.DeleteRange(oldPF.IDImages.AsQueryable());
                dlImgRepo.DeleteRange(oldPF.DriverLicenseImages.AsQueryable());

                pfRepo.Delete(oldPF);
            }

            
            var user = await userRepo.GetByIdAsync(newPF.UserID);
            user.IsVerified = 3;

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectProfileAsync(int renterId)
        {
            var pfRepo = _unitOfWork.Repository<RenterProfile>();
            var userRepo = _unitOfWork.Repository<User>();
            var idImgRepo = _unitOfWork.Repository<IDImage>();
            var dlImgRepo = _unitOfWork.Repository<DriverLicenseImage>();
            var imageRepo = _unitOfWork.Repository<Image>();

            
            var newPF = await pfRepo.AsQueryable()
                .Where(p => p.UserID == renterId && !p.IsDelete)
                .Include(p => p.IDImages).ThenInclude(i => i.Image)
                .Include(p => p.DriverLicenseImages).ThenInclude(i => i.Image)
                .FirstOrDefaultAsync();

            if (newPF == null)
                throw new KeyNotFoundException("Profile not found.");

            var user = await userRepo.GetByIdAsync(newPF.UserID);

           
            var oldPF = await pfRepo.AsQueryable()
                .Where(p => p.UserID == newPF.UserID && p.IsDelete)
                .FirstOrDefaultAsync();


            
            if (oldPF == null)
            {
                
                foreach (var img in newPF.IDImages)
                    imageRepo.Delete(img.Image);

                foreach (var img in newPF.DriverLicenseImages)
                    imageRepo.Delete(img.Image);

                idImgRepo.DeleteRange(newPF.IDImages.AsQueryable());
                dlImgRepo.DeleteRange(newPF.DriverLicenseImages.AsQueryable());

                pfRepo.Delete(newPF);

                
                user.IsVerified = 1;

                await _unitOfWork.SaveChangesAsync();
                return true;
            }


            
            foreach (var img in newPF.IDImages)
                imageRepo.Delete(img.Image);

            foreach (var img in newPF.DriverLicenseImages)
                imageRepo.Delete(img.Image);

            idImgRepo.DeleteRange(newPF.IDImages.AsQueryable());
            dlImgRepo.DeleteRange(newPF.DriverLicenseImages.AsQueryable());
            pfRepo.Delete(newPF);

            
            oldPF.IsDelete = false;

            
            user.IsVerified = 3;

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<UploadRPResponseModel> GetProfileByUserIdAsync(int userId)
        {
            var user = await _unitOfWork.Repository<User>()
                .AsQueryable()
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDelete);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            var pf = await _unitOfWork.Repository<RenterProfile>()
                .AsQueryable()
                .Where(p => p.UserID == userId && !p.IsDelete)
                .Include(p => p.IDImages).ThenInclude(i => i.Image)
                .Include(p => p.DriverLicenseImages).ThenInclude(i => i.Image)
                .FirstOrDefaultAsync();

            
            if (pf == null)
            {
                return new UploadRPResponseModel
                {
                    RenterId = userId,
                    VerificationStatus = 1,
                    Message = "No profile exists for this user."
                };
            }

            return new UploadRPResponseModel
            {
                RenterId = pf.UserID,
                IDNumber = pf.IDNumber,
                DriverLicenseNo = pf.DriverLicenseNo,

                IDCardFrontImage = pf.IDImages.FirstOrDefault(i => i.Type == 1)?.Image?.Base64Image,
                IDCardBackImage = pf.IDImages.FirstOrDefault(i => i.Type == 2)?.Image?.Base64Image,

                DriverLicenseFrontImage = pf.DriverLicenseImages.FirstOrDefault(i => i.Type == 1)?.Image?.Base64Image,
                DriverLicenseBackImage = pf.DriverLicenseImages.FirstOrDefault(i => i.Type == 2)?.Image?.Base64Image,

                VerificationStatus = user.IsVerified,
                Message = user.IsVerified switch
                {
                    1 => "No profile created.",
                    2 => "Profile pending staff approval.",
                    3 => "Profile has been approved.",
                    _ => "Profile retrieved."
                }
            };
        }

    }
}
