﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Pulsar4X.ECSLib
{
    public static class Misc
    {
        public static bool HasReqiredItems(Dictionary<Guid, int> stockpile, Dictionary<Guid, int> costs)
        {
            if (costs == null)
                return true;
            return costs.All(kvp => stockpile.ContainsKey(kvp.Key) && (stockpile[kvp.Key] >= kvp.Value));
        }

        public static void UseFromStockpile(Dictionary<Guid, int> stockpile, Dictionary<Guid, int> costs)
        {
            if (costs != null)
            {
                foreach (var kvp in costs)
                {
                    stockpile[kvp.Key] -= kvp.Value;
                }
            }
        }

        /// <summary>
        /// Values the hash.
        /// </summary>
        /// <returns>The hash.</returns>
        /// <param name="obj">Object.</param>
        /// <param name="hash">if mulitiple values need to be hashed, include the previous hash</param>
        public static int ValueHash(object obj, int hash = 17)
        {
            if (obj != null)
            {
                unchecked { hash = hash * 31 + obj.GetHashCode(); }
            }            
            return hash;
        }

        /// <summary>
        /// A horribly inefficent way of getting all the entities owned by a specific faction in a manager. 
        /// TODO: maybe do ownership at a higher (or lower? whichever way around) level, ie not a datablob on entities. 
        /// </summary>
        /// <returns>The entitys in manager.</returns>
        /// <param name="factionOwner">Faction owner.</param>
        /// <param name="manager">Manager.</param>
        public static List<Entity> OwnedEntitysInManager(Entity factionOwner, EntityManager manager)
        {
            var owner = factionOwner.GetDataBlob<FactionOwnerDB>();
            List<Entity> listOfOwnedEntites = new List<Entity>();
            foreach (var item in manager.GetAllEntitiesWithDataBlob<OwnedDB>())
            {
                if (owner.OwnedEntities.ContainsKey(item.Guid))
                    listOfOwnedEntites.Add(item);
                    
            }
            return listOfOwnedEntites;
        }

    }

    public static class DictionaryExtension
    {
        /// <summary>
        /// Adds an int value to a dictionary, adding the key if the key does not exsist.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="toReplace"></param>
        [PublicAPI]
        public static void SafeValueReplace<TKey>(this Dictionary<TKey, int> dict, TKey key, int toReplace)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, toReplace);
            else
            {
                dict[key] = toReplace;
            }
        }

        public static void SafeValueReplace<TKey>(this Dictionary<TKey, long> dict, TKey key, long toReplace)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, toReplace);
            else
            {
                dict[key] = toReplace;
            }
        }

        /// <summary>
        /// Adds an int value to a dictionary, adding the key if the key does not exsist.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="toAdd"></param>
        [PublicAPI]
        public static void SafeValueAdd<TKey>(this Dictionary<TKey, int> dict, TKey key, int toAdd)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, toAdd);
            else
            {
                dict[key] += toAdd;
            }
        }

        /// <summary>
        /// Adds an long value to a dictionary, adding the key if the key does not exsist.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="toAdd"></param>
        [PublicAPI]
        public static void SafeValueAdd<TKey>(this Dictionary<TKey, long> dict, TKey key, long toAdd)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, toAdd);
            else
            {
                dict[key] += toAdd;
            }
        }

        /// <summary>
        /// Adds a float value to a dictionary, adding the key if the key does not exsist.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="toAdd"></param>
        [PublicAPI]
        public static void SafeValueAdd<TKey>(this Dictionary<TKey, float> dict, TKey key, float toAdd)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, toAdd);
            else
            {
                dict[key] += toAdd;
            }
        }
        /// <summary>
        /// Adds a double value to a dictionary, adding the key if the key does not exsist.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="toAdd"></param>
        [PublicAPI]
        public static void SafeValueAdd<TKey>(this Dictionary<TKey, double> dict, TKey key, double toAdd)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, toAdd);
            else
            {
                dict[key] += toAdd;
            }
        }
    }

    public static class ListExtension
    {
        [PublicAPI]
        public static List<Entity> GetEntititiesWithDataBlob<TDataBlob>(this List<Entity> list) where TDataBlob : BaseDataBlob
        {
            var retVal = new List<Entity>();
            foreach (Entity entity in list)
            {
                if (entity.HasDataBlob<TDataBlob>())
                {
                    retVal.Add(entity);
                }
            }

            return retVal;
        }
    }

}
