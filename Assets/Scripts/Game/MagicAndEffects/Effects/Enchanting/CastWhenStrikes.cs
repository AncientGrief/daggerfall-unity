// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using System.Collections.Generic;
using DaggerfallConnect.Save;
using DaggerfallConnect.FallExe;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Cast spell on target when weapon strikes.
    /// </summary>
    public class CastWhenStrikes : BaseEntityEffect
    {
        public static readonly string EffectKey = EnchantmentTypes.CastWhenStrikes.ToString();

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.GroupName = TextManager.Instance.GetText(textDatabase, EffectKey);
            properties.AllowedCraftingStations = MagicCraftingStations.ItemMaker;
            properties.ItemMakerFlags = ItemMakerFlags.AllowMultiplePrimaryInstances | ItemMakerFlags.AlphaSortSecondaryList;
        }

        /// <summary>
        /// Outputs spells available to this item effect abstracted as EnchantmentSettings array.
        /// When EnchantmentSettings.ClassicParam is set, it refers to a classic spell ID.
        /// When EnchantmentSettings.CustomParam is set, it refers to a custom spell bundle.
        /// </summary>
        public override EnchantmentSettings[] GetEnchantmentSettings()
        {
            List<EnchantmentSettings> enchantments = new List<EnchantmentSettings>();

            // Enumerate classic spells
            SpellRecord.SpellRecordData spellRecord;
            for(int i = 0; i < classicSpellIDs.Length; i++)
            {
                short id = classicSpellIDs[i];
                if (!GameManager.Instance.EntityEffectBroker.GetClassicSpellRecord(id, out spellRecord))
                    throw new Exception(string.Format("Could not find classic spell record with ID '{0}'", id));

                EnchantmentSettings enchantment = new EnchantmentSettings()
                {
                    Version = 1,
                    EffectKey = EffectKey,
                    ClassicType = EnchantmentTypes.CastWhenStrikes,
                    ClassicParam = id,
                    PrimaryDisplayName = properties.GroupName,
                    SecondaryDisplayName = spellRecord.spellName,
                    EnchantCost = classicSpellCosts[i],
                };

                enchantments.Add(enchantment);
            }

            // Enumerate custom spell bundle offers supporting CastWhenStrikesEnchantment flag
            EntityEffectBroker.CustomSpellBundleOffer[] offers = GameManager.Instance.EntityEffectBroker.GetCustomSpellBundleOffers(EntityEffectBroker.CustomSpellBundleOfferUsage.CastWhenStrikesEnchantment);
            foreach(EntityEffectBroker.CustomSpellBundleOffer offer in offers)
            {
                EnchantmentSettings enchantment = new EnchantmentSettings()
                {
                    Version = 1,
                    EffectKey = EffectKey,
                    CustomParam = offer.Key,
                    PrimaryDisplayName = properties.GroupName,
                    SecondaryDisplayName = offer.BundleSetttings.Name,
                    EnchantCost = offer.EnchantmentCost,
                };

                enchantments.Add(enchantment);
            }

            return enchantments.ToArray();
        }

        #region Classic Support

        // Classic spell IDs available for this effect are hard-coded and automatically populated

        static short[] classicSpellIDs =
        {
            50,     //Paralysis
            53,     //Hand of Sleep
            52,     //Vampiric Touch
            54,     //Magicka Leech
            56,     //Hand of Decay
            33,     //Wildfire
            20,     //Ice Storm
            25,     //Fire Storm
            16,     //Ice Bolt
            7,      //Wizard's Fire
            55,     //Sphere of Negation
            67,     //Energy Leech
        };

        static short[] classicSpellCosts =
        {
            1620,   //Paralysis
            780,    //Hand of Sleep
            1380,   //Vampiric Touch
            930,    //Magicka Leech
            1830,   //Hand of Decay
            1020,   //Wildfire
            840,    //Ice Storm
            840,    //Fire Storm
            990,    //Ice Bolt
            480,    //Wizard's Fire
            4230,   //Sphere of Negation
            1260,   //Energy Leech
        };

        #endregion
    }
}