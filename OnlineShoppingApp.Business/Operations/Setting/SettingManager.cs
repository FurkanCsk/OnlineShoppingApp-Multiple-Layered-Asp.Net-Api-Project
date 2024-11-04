using OnlineShoppingApp.Data.Entities;
using OnlineShoppingApp.Data.Repositories;
using OnlineShoppingApp.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingApp.Business.Operations.Setting
{
    public class SettingManager : ISettingService
    {
        private readonly IUnitOfWork _unitOfWork; // Manages database transactions
        private readonly IRepository<SettingEntity> _settingRepository; // Repository for setting data

        public SettingManager(IUnitOfWork unitOfWork, IRepository<SettingEntity> settingRepository)
        {
            _settingRepository = settingRepository; // Initialize settings repository
            _unitOfWork = unitOfWork; // Initialize unit of work
        }

        public bool GetMaintenanceState()
        {
            // Retrieve the maintenance mode state
            var maintenanceState = _settingRepository.GetById(1).MaintenanceMode;

            return maintenanceState; // Return the state
        }

        public async Task ToggleMaintenance()
        {
            var setting = _settingRepository.GetById(1); // Get the settings entity

            setting.MaintenanceMode = !setting.MaintenanceMode; // Toggle maintenance mode

            _settingRepository.Update(setting); // Mark setting as updated

            try
            {
                await _unitOfWork.SaveChangesAsync(); // Save changes to the database
            }
            catch (Exception)
            {

                throw new Exception("An error occurred while updating the maintenance status.");
            }
        }
    }
}
