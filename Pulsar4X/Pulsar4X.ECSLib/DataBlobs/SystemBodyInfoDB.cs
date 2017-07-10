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
    public enum BodyType : byte
    {
        Terrestrial, // Like Earth/Mars/Venus/etc.
        GasGiant, // Like Jupiter/Saturn
        IceGiant, // Like Uranus/Neptune
        DwarfPlanet, // Pluto!
        GasDwarf, // What you'd get is Jupiter and Saturn ever had a baby.

        /// < @todo Add more planet types like Ice Planets ( bigger Plutos), carbon planet ( http:// en.wikipedia.org/ wiki/ Carbon_planet), Iron SystemBody ( http:// en.wikipedia.org/ wiki/ Iron_planet) or Lava Planets ( http:// en.wikipedia.org/ wiki/ Lava_planet). ( more: http:// en.wikipedia.org/ wiki/ List_of_planet_types
        /// )
        /// .
        Moon,
        Asteroid,
        Comet
    }

    public enum TectonicActivity : byte
    {
        Dead,
        Minor,
        EarthLike,
        Major,
        NA
    }

    /// <summary>
    /// Small struct to store specifics of a minerial deposit.
    /// </summary>
    public class MineralDepositInfo
    {
        #region Properties
        [JsonProperty]
        public int Amount { get; set; }
        [JsonProperty]
        public int HalfOriginalAmount { get; set; }
        [JsonProperty]
        public double Accessibility { get; set; }
        #endregion
    }

    /// <summary>
    /// SystemBodyInfoDB defines an entity as having properties like planets/asteroids/coments.
    /// </summary>
    /// <remarks>
    /// Specifically, Minerals, body info, atmosphere info, and gravity.
    /// </remarks>
    public class SystemBodyInfoDB : BaseDataBlob
    {
        #region Fields
        private float _atmosphericDust;
        private float _axialTilt;
        private float _baseTemperature;
        private BodyType _bodyType;
        private double _gravity;
        private TimeSpan _lengthOfDay;
        private float _magneticField;
        private float _radiationLevel;
        private bool _supportsPopulations;
        private TectonicActivity _tectonics;
        #endregion

        #region Properties
        /// <summary>
        /// Type of body this is. <see cref="BodyType" />
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public BodyType BodyType
        {
            get { return _bodyType; }
            set
            {
                SetField(ref _bodyType, value);
                ;
            }
        }

        /// <summary>
        /// Plate techtonics. Ammount of activity depends on age vs mass.
        /// Influences magnitic field. maybe this should be in the processor?
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public TectonicActivity Tectonics
        {
            get { return _tectonics; }
            set
            {
                SetField(ref _tectonics, value);
                ;
            }
        }

        /// <summary>
        /// The Axial Tilt of this body.
        /// Measured in degrees.
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public float AxialTilt
        {
            get { return _axialTilt; }
            set
            {
                SetField(ref _axialTilt, value);
                ;
            }
        }

        /// <summary>
        /// Magnetic field of the body. It is important as it affects how much atmosphere a body will have.
        /// In Microtesla (uT)
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public float MagneticField
        {
            get { return _magneticField; }
            set
            {
                SetField(ref _magneticField, value);
                ;
            }
        }

        /// <summary>
        /// Temperature of the planet BEFORE greenhouse effects are taken into consideration.
        /// This is mostly a factor of how much light reaches the planet nad is calculated at generation time.
        /// In Degrees C.
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public float BaseTemperature
        {
            get { return _baseTemperature; }
            set
            {
                SetField(ref _baseTemperature, value);
                ;
            }
        }

        /// <summary>
        /// Amount of radiation on this body. Affects ColonyCost.
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public float RadiationLevel
        {
            get { return _radiationLevel; }
            set
            {
                SetField(ref _radiationLevel, value);
                ;
            }
        }

        /// <summary>
        /// Amount of atmosphic dust on this body. Affects ColonyCost.
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public float AtmosphericDust
        {
            get { return _atmosphericDust; }
            set
            {
                SetField(ref _atmosphericDust, value);
                ;
            }
        }

        /// <summary>
        /// Indicates if the system body supports populations and can be settled by Players/NPRs.
        /// </summary>
        /// <remarks>
        /// TODO: Gameplay Review
        /// See if we want to decide SupportsPopulations some other way.
        /// Some species may be capable of habiting different types and different bodies.
        /// Maybe this should be at the species level.
        /// </remarks>
        [PublicAPI]
        [JsonProperty]
        public bool SupportsPopulations
        {
            get { return _supportsPopulations; }
            set
            {
                SetField(ref _supportsPopulations, value);
                ;
            }
        }

        /// <summary>
        /// Length of day for this body. Mostly fluff.
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public TimeSpan LengthOfDay
        {
            get { return _lengthOfDay; }
            set
            {
                SetField(ref _lengthOfDay, value);
                ;
            }
        }

        /// <summary>
        /// Gravity on this body measured in m/s/s. Affects ColonyCost.
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public double Gravity
        {
            get { return _gravity; }
            set
            {
                SetField(ref _gravity, value);
                ;
            }
        }

        /// <summary>
        /// Stores the amount of the variopus minerials. the guid can be used to lookup the
        /// minerial definition (MineralSD) from the StaticDataStore.
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public ObservableDictionary<Guid, MineralDepositInfo> Minerals { get; set; } = new ObservableDictionary<Guid, MineralDepositInfo>();
        #endregion

        #region Constructors
        public SystemBodyInfoDB() { Minerals.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(Minerals), args); }

        public SystemBodyInfoDB(SystemBodyInfoDB systemBodyDB) : this()
        {
            BodyType = systemBodyDB.BodyType;
            Tectonics = systemBodyDB.Tectonics;
            AxialTilt = systemBodyDB.AxialTilt;
            MagneticField = systemBodyDB.MagneticField;
            BaseTemperature = systemBodyDB.BaseTemperature;
            RadiationLevel = systemBodyDB.RadiationLevel;
            AtmosphericDust = systemBodyDB.AtmosphericDust;
            SupportsPopulations = systemBodyDB.SupportsPopulations;
            LengthOfDay = systemBodyDB.LengthOfDay;
            Gravity = systemBodyDB.Gravity;
            Minerals.Merge(systemBodyDB.Minerals);
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new SystemBodyInfoDB(this);
        #endregion
    }
}