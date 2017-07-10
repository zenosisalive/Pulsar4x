﻿#region Copyright/License
/* 
 *Copyright© 2017 Daniel Phelps
    This file is part of Pulsar4x.

    Pulsar4x is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Pulsar4x is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Pulsar4x.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

using System;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public class CargoStorageAtbDB : BaseDataBlob
    {
        #region Fields
        private Guid _cargoTypeGuid;
        private int _storageCapacity;
        #endregion

        #region Properties
        /// <summary>
        /// Storage Capacity of this module.
        /// </summary>
        [JsonProperty]
        public int StorageCapacity { get { return _storageCapacity; } set { SetField(ref _storageCapacity, value); } }

        /// <summary>
        /// Type of cargo this stores
        /// </summary>
        [JsonProperty]
        public Guid CargoTypeGuid { get { return _cargoTypeGuid; } set { SetField(ref _cargoTypeGuid, value); } }
        #endregion

        #region Constructors
        //public CargoTypeSD CargoType { get; private set; }

        /// <summary>
        /// JSON constructor
        /// </summary>
        public CargoStorageAtbDB() { }

        /// <summary>
        /// Parser Constructor
        /// </summary>
        /// <param name="storageCapacity">will get cast to an int</param>
        /// <param name="cargoType">cargo type ID as defined in StaticData CargoTypeSD</param>
        public CargoStorageAtbDB(double storageCapacity, Guid cargoType) : this((int)storageCapacity, cargoType) { }

        public CargoStorageAtbDB(int storageCapacity, Guid cargoType)
        {
            StorageCapacity = storageCapacity;
            CargoTypeGuid = cargoType;
        }

        public CargoStorageAtbDB(CargoStorageAtbDB db)
        {
            StorageCapacity = db.StorageCapacity;
            CargoTypeGuid = db.CargoTypeGuid;
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new CargoStorageAtbDB(this);
        #endregion
    }
}