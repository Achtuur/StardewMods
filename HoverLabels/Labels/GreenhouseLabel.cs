using HoverLabels.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Buildings;
using AchtuurCore.Extensions;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;
using StardewValley.Objects;

namespace HoverLabels.Labels;
internal class GreenhouseLabel : BaseLabel
{
    /// <summary>
    /// Number of items that are shown in label before cutting off
    /// </summary>
    internal const int LabelListSize = 5;
    GreenhouseBuilding greenHouse;
    Farm greenHouseFarm;

    public GreenhouseLabel(int? priority=null): base(priority)
    {
    }

    public override void SetCursorTile(Vector2 cursorTile)
    {
        this.CursorTile = cursorTile;
        greenHouse = Game1.getFarm().buildings.Where(b => b is GreenhouseBuilding).First() as GreenhouseBuilding;

        if (greenHouse is not null)
            this.greenHouseFarm = this.greenHouse.GetFarm();
    }

    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        // Assume that greenhouse can only be on farm and outdoors
        if (!Game1.currentLocation.IsFarm || !Game1.currentLocation.IsOutdoors)
            return false;

        GreenhouseBuilding greenHouse = Game1.getFarm().buildings.Where(b => b is GreenhouseBuilding).First() as GreenhouseBuilding;
        return greenHouse is not null && greenHouse.GetRect().Contains(cursorTile);
    }

    public override void GenerateLabel()
    {
        this.Name = I18n.LabelGreenhouseName();
        GenerateCropInfoLabel();
    }

    private void GenerateCropInfoLabel()
    {
        List<CropInfo> cropInfo = GetGreenhouseCropInfo().ToList();
        if (cropInfo.Count <= 0)
            return;

        GenerateReadyCropsLabel(cropInfo);
        GenerateGrowingCropsLabel(cropInfo);
        GenerateFruittreeLabel();
    }

    private void GenerateFruittreeLabel()
    {
        IEnumerable<FruitTree> fullyGrownFruitTrees = GetGreenhouseTerrainFeature<FruitTree>()
            .Where(tree => tree.growthStage.Value == 4);

        if (fullyGrownFruitTrees.Count() <= 0)
            return;

        // Sort tree by number of fruit -> name
        fullyGrownFruitTrees = fullyGrownFruitTrees
            .OrderByDescending(tree => tree.fruitsOnTree.Value)
            .ThenBy(tree => ModEntry.GetObjectWithId(tree.indexOfFruit.Value).DisplayName);

        this.Description.Add("Fruit Trees:");
        foreach (FruitTree fruitTree in fullyGrownFruitTrees.Take(ModEntry.Instance.Config.LabelListMaxSize))
        {
            SObject fruitObj = ModEntry.GetObjectWithId(fruitTree.indexOfFruit.Value);
            this.Description.Add(I18n.LabelGreenhouseFruittrees(fruitObj.DisplayName, fruitTree.fruitsOnTree.Value));
        }
    }

    private void GenerateReadyCropsLabel(List<CropInfo> greenhouseCrops)
    {
        // Generate label for crops that are ready
        IEnumerable<CropInfo> readyCrops = greenhouseCrops.Where(c => c.fullyGrown)
            .OrderBy(c => c.name);

        if (readyCrops.Count() > 0)
        {
            Dictionary<CropInfo, int> readyCropAmount = GetCropAmountDictionary(readyCrops);
            this.Description.Add(I18n.LabelGreenhouseHarvestableCrops());
            foreach ((CropInfo crop, int amount) in readyCropAmount.Select(x => (x.Key, x.Value)).Take(LabelListSize))
            {
                this.Description.Add($"> {crop.name}: {amount}");
            }

            if (readyCropAmount.Count() > LabelListSize)
                this.Description.Add(I18n.LabelGreenhouseOtherCrops(readyCropAmount.Count() - LabelListSize));
        }
    }
    
    private void GenerateGrowingCropsLabel(List<CropInfo> greenhouseCrops)
    {
        // Generate label for growing crops
        IEnumerable<CropInfo> growingCrops = greenhouseCrops.Where(c => !c.fullyGrown)
            .OrderBy(c => c.days_to_grow)
            .ThenBy(c => c.name);

        if (growingCrops.Count() > 0)
        {
            Dictionary<CropInfo, int> growingCropAmount = GetCropAmountDictionary(growingCrops);
            this.Description.Add(I18n.LabelGreenhouseCropsReadyIn());
            foreach ((CropInfo crop, int amount) in growingCropAmount.Take(LabelListSize))
            {
                string cropReadyDate = ModEntry.GetDateAfterDays(crop.days_to_grow);
                this.Description.Add(I18n.LabelGreenhouseCropGrowTime(amount, crop.name, crop.days_to_grow, cropReadyDate));
            }

            if (growingCropAmount.Count() > LabelListSize)
                this.Description.Add(I18n.LabelGreenhouseOtherCrops(growingCropAmount.Count() - LabelListSize));
            
        }
    }

    /// <summary>
    /// Returns a dictionary wíth all unique <see cref="CropInfo"/> in <paramref name="cropInfo"/> as keys, and occurences as the value
    /// </summary>
    /// <param name="cropInfo"></param>
    /// <returns></returns>
    private static Dictionary<CropInfo, int> GetCropAmountDictionary(IEnumerable<CropInfo> cropInfo)
    {
        // Make dictionary to get unique names with amounts for each crop
        Dictionary<CropInfo, int> cropAmount = new Dictionary<CropInfo, int>();
        foreach (CropInfo crop in cropInfo)
        {
            if (!cropAmount.ContainsKey(crop))
                cropAmount.Add(crop, 0);
            cropAmount[crop]++;
        }
        return cropAmount;
    }

    private static IEnumerable<CropInfo> GetGreenhouseCropInfo()
    {
        // Crops on hoeDirt
        foreach (HoeDirt hoeDirt in GetGreenhouseTerrainFeature<HoeDirt>())
        {

            // Should have crop in greenhouse
            if (!hoeDirt.isGreenhouseDirt.Value || hoeDirt.crop is null || hoeDirt.crop.dead.Value)
                continue;

            yield return CropInfo.from_crop(hoeDirt.crop);
        }

        // Crops inside garden pots
        foreach (SObject sobj in GetGreenhouseObjects())
        {
            if (sobj is not IndoorPot pot)
                continue;

            yield return CropInfo.from_crop(pot.hoeDirt.Value.crop);
        }
    }

    public static IEnumerable<T> GetGreenhouseTerrainFeature<T>()
    {
        return Game1.locations
            .Where(loc => loc.IsGreenhouse)
            .First().terrainFeatures.Values //take first greenhouse location's terrainfeatures
            .Where(feature => feature is T).Cast<T>(); //cast to T
    }

    public static IEnumerable<SObject> GetGreenhouseObjects()
    {
        return Game1.locations.Where(loc => loc.IsGreenhouse).First().Objects.Values;
    }

    private record struct CropInfo(string name, int days_to_grow, bool fullyGrown)
    {
        /// <summary>
        /// Returns a <see cref="CropInfo"/> from a <see cref="StardewValley.Crop"/>.
        /// </summary>
        /// <param name="crop"></param>
        /// <returns></returns>
        public static CropInfo from_crop(Crop crop) => new CropInfo(
            name: CropLabel.GetCropAsObject(crop).DisplayName,
            days_to_grow: CropLabel.GetDaysUntilFullyGrown(crop),
            fullyGrown: CropLabel.IsCropFullyGrown(crop)
        );
    }
}

