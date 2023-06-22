using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerExpShare.Integrations
{
    /// <summary>Implementation of the mod API.</summary>
    public interface IModularOverhaulApi
    { 
        /// <summary>Sets a flag to allow the specified SpaceCore skill to level past 10 and offer prestige professions.</summary>
        /// <param name="id">The SpaceCore skill id.</param>
        /// <remarks>
        ///     All this does is increase the level cap for the skill with the specified <paramref name="id"/>.
        ///     The custom Skill mod author is responsible for making sure their professions return the correct
        ///     description and icon when prestiged. To check if a <see cref="Farmer"/> instance has a given prestiged
        ///     profession, simply add 100 to the profession's base ID.
        /// </remarks>
        void RegisterCustomSkillForPrestige(string id);
    }
}