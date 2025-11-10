using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.Core;
using EVRenter_Data.Entities;
using EVRenter_Repository.UnitOfWork;
using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.Service
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleResponseModel>> GetAllVehicle();
        Task<VehicleResponseModel?> GetVehicleByIdAsync(int id);
        Task<VehicleResponseModel> CreateVehicleAsync(VehicleRequestModel request);
        Task<VehicleResponseModel?> UpdateVehicleAsync(int id, VehicleUpdateRequest request);
        Task<VehicleResponseModel?> UpdateVehicleStatusAsync(int vehicleId);
        Task<bool> DeleteVehicleAsync(int id);
    }
    public class VehicleService : IVehicleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public VehicleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<VehicleResponseModel>> GetAllVehicle()
        {
            return await _unitOfWork.Repository<Vehicle>()
                .GetQueryable()
                .Where(x => !x.IsDelete)
                .ProjectTo<VehicleResponseModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<VehicleResponseModel?> GetVehicleByIdAsync(int id)
        {
            // Get the user with basic information
            var vehicle = await _unitOfWork.Repository<Vehicle>().AsQueryable()
                .Where(u => u.Id == id)
                .ProjectTo<VehicleResponseModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return vehicle;
        }

        public async Task<VehicleResponseModel> CreateVehicleAsync(VehicleRequestModel request)
        {
            if (request == null)
                throw new ArgumentException("Invalid request data.");

            var model = await _unitOfWork.Repository<Model>().AsQueryable()
                .Where(u => u.Id == request.ModelID && !u.IsDelete)
                .FirstOrDefaultAsync();
            if (model == null)
            {
                throw new Exception("Model not found");
            }

            var station = await _unitOfWork.Repository<Station>().AsQueryable()
                .Where(u => u.Id == request.StationID && !u.IsDelete)
                .FirstOrDefaultAsync();
            if (station == null)
            {
                throw new Exception("Station not found");
            }

            var existingVehicle = await _unitOfWork.Repository<Vehicle>().AsQueryable().
                Where(u => u.PlateNumber == request.PlateNumber && !u.IsDelete).FirstOrDefaultAsync();
            if (existingVehicle != null)
            {
                throw new InvalidOperationException("Plate Number already exists.");
            }

            var vehicle = _mapper.Map<Vehicle>(request);
            await _unitOfWork.Repository<Vehicle>().InsertAsync(vehicle);

            model.Quantity++;
            await _unitOfWork.Repository<Model>().UpdateAsync(model);

            await _unitOfWork.SaveChangesAsync();


            //
            var categories = await _unitOfWork.Repository<ItemCategory>().AsQueryable().
                Where(c => !c.IsDelete).ToListAsync();
            var defaultChecklist = new List<CarItem>();

            foreach (var category in categories)
            {
                if (DefaultCarChecklist.Checklist.TryGetValue(category.Name, out var items))
                {
                    foreach (var name in items)
                    {
                        defaultChecklist.Add(new CarItem
                        {
                            VehicleID = vehicle.Id,
                            CategoryID = category.Id,
                            Name = name,
                            Status = 1 // 0 = "miss"
                        });
                    }
                }
            }

            await _unitOfWork.Repository<CarItem>().AddRangeAsync(defaultChecklist);
            await _unitOfWork.SaveChangesAsync();


            var createdVehicle = await _unitOfWork.Repository<Vehicle>()
                .AsQueryable()
                .Where(x => x.Id == vehicle.Id)
                .ProjectTo<VehicleResponseModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (createdVehicle == null)
            {
                throw new Exception("Failed to retrieve created Vehicle.");
            }

            return createdVehicle;
        }

        public async Task<VehicleResponseModel?> UpdateVehicleAsync(int id, VehicleUpdateRequest request)
        {
            var existingVehicle = await _unitOfWork.Repository<Vehicle>()
                .AsQueryable()
                .Where(s => s.Id == id && !s.IsDelete)
                .FirstOrDefaultAsync();

            if (existingVehicle == null) return null;

            // Kiểm tra xem có bất kỳ trường nào được cập nhật không
            bool hasUpdates = false;

            // Cập nhật từng trường nếu có giá trị mới
            if (!string.IsNullOrEmpty(request.PlateNumber))
            {
                existingVehicle.PlateNumber = request.PlateNumber;
                hasUpdates = true;
            }
            if (request.ModelID.HasValue)
            {
                var model = await _unitOfWork.Repository<Model>().AsQueryable()
                .Where(u => u.Id == request.ModelID.Value)
                .FirstOrDefaultAsync();
                if (model == null)
                {
                    throw new Exception("Model not found");
                }

                existingVehicle.ModelID = request.ModelID.Value;
                hasUpdates = true;
            }

            if (request.StationID.HasValue)
            {
                var station = await _unitOfWork.Repository<Station>().AsQueryable()
                    .Where(u => u.Id == request.StationID.Value)
                    .FirstOrDefaultAsync();
                if (station == null)
                {
                    throw new Exception("Station not found");
                }

                existingVehicle.StationID = request.StationID.Value;
                hasUpdates = true;
            }

            if (!string.IsNullOrEmpty(request.Location))
            {
                existingVehicle.Location = request.Location;
                hasUpdates = true;
            }

            if (request.BatteryLevel.HasValue)
            {
                existingVehicle.BatteryLevel = request.BatteryLevel.Value;
                hasUpdates = true;
            }

            if (request.Odometer.HasValue)
            {
                existingVehicle.Odometer = request.Odometer.Value;
                hasUpdates = true;
            }

            if (request.Status.HasValue)
            {
                existingVehicle.Status = request.Status.Value;
                hasUpdates = true;
            }

            if (!string.IsNullOrEmpty(request.Color))
            {
                existingVehicle.Color = request.Color;
                hasUpdates = true;
            }

            if (hasUpdates)
            {
                await _unitOfWork.Repository<Vehicle>().Update(existingVehicle, id);
                await _unitOfWork.SaveChangesAsync();
            }

            return _mapper.Map<VehicleResponseModel>(existingVehicle);
        }

        public async Task<VehicleResponseModel?> UpdateVehicleStatusAsync(int vehicleId)
        {
            var existingVehicle = await _unitOfWork.Repository<Vehicle>()
                .AsQueryable()
                .Where(s => s.Id == vehicleId && !s.IsDelete && s.Status < 5)
                .FirstOrDefaultAsync();
            if (existingVehicle == null || existingVehicle.Status == 0) return null;

            var existingBooking = await _unitOfWork.Repository<Booking>()
                .AsQueryable()
                .Where(s => s.VehicleID == vehicleId && !s.IsDelete && s.Status < 3)
                .FirstOrDefaultAsync();
            if (existingBooking == null) return null;

            if (existingVehicle.Status > 0 && existingVehicle.Status < 3)
            {

                existingVehicle.Status++;
                existingBooking.Status++;
            }
            else if (existingVehicle.Status == 3)
            {
                existingVehicle.Status++;
            }
            else if (existingVehicle.Status == 4)
            {
                existingVehicle.Status = 0;
                existingBooking.Status = 3;
            }


            await _unitOfWork.Repository<Vehicle>().Update(existingVehicle, vehicleId);
            await _unitOfWork.Repository<Booking>().Update(existingBooking, existingBooking.Id);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<VehicleResponseModel>(existingVehicle);
        }

        public async Task<bool> ResetBookingOfVehicle()
        {
            var vehicles = await _unitOfWork.Repository<Vehicle>()
                .GetQueryable()
                .Where(x => !x.IsDelete && x.Status != 0)
                .ToListAsync();
            if (!vehicles.Any()) return false;

            var bookings = await _unitOfWork.Repository<Booking>()
                .GetQueryable()
                .Where(x => !x.IsDelete)
                .ProjectTo<BookingResponseModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
            if (!bookings.Any()) return false;

            foreach (var vehicle in vehicles)
            {
                vehicle.Status = 0;
                await _unitOfWork.Repository<Vehicle>().InsertAsync(vehicle);
            }


            //await _unitOfWork.Repository<Vehicle>().Update(existingVehicle, vehicleId);
            //await _unitOfWork.Repository<Booking>().Update(existingBooking, existingBooking.Id);
            //await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteVehicleAsync(int id)
        {
            var vehicle = await _unitOfWork.Repository<Vehicle>().GetById(id);
            if (vehicle == null) return false;

            vehicle.IsDelete = true;

            var model = await _unitOfWork.Repository<Model>().AsQueryable()
                .Where(u => u.Id == vehicle.ModelID && !u.IsDelete)
                .FirstOrDefaultAsync();
            if (model == null)
            {
                throw new Exception("Model not found");
            }
            model.Quantity--;

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }

    public static class DefaultCarChecklist
    {
        public static readonly Dictionary<string, string[]> Checklist = new()
        {
            ["Ngoại thất"] = new[]
            {
            "Đèn pha trước", "Đèn hậu", "Gương chiếu hậu", "Lốp xe 4 bánh",
            "Thân xe (trầy xước)", "Cửa xe (4 cửa)", "Cốp sau"
        },
            ["Nội thất"] = new[]
            {
            "Ghế lái & ghế phụ", "Ghế hàng 2", "Dây đai an toàn",
            "Điều hòa không khí", "Hệ thống âm thanh", "Màn hình trung tâm"
        },
            ["Pin & Kỹ thuật"] = new[]
            {
            "Mức pin hiện tại", "Cổng sạc", "Hệ thống phanh",
            "Đèn báo trên táp-lô", "Hệ thống lái"
        },
            ["Phụ kiện"] = new[]
            {
            "Dây sạc di động", "Sách hướng dẫn", "Chìa khóa (2 chiếc)", "Bộ dụng cụ sơ cứu"
        }
        };
    }

}