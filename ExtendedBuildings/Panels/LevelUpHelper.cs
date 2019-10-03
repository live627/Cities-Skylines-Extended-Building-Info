using ColossalFramework;
using System;

namespace ExtendedBuildings
{
    class LevelUpHelper3
    {
        private static LevelUpHelper3 m_instance;
        public static LevelUpHelper3 instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new LevelUpHelper3();
                return m_instance;
            }
        }

        public double GetPollutionFactor(ItemClass.Zone zone)
            => zone == ItemClass.Zone.ResidentialHigh || zone == ItemClass.Zone.ResidentialLow
                ? -0.2 : zone == ItemClass.Zone.Office ? -0.25 : -0.1667;

        public bool IsResourcePositive(ImmaterialResourceManager.Resource resource) => resource != ImmaterialResourceManager.Resource.Abandonment
                || resource != ImmaterialResourceManager.Resource.CrimeRate
                || resource != ImmaterialResourceManager.Resource.FireHazard
                || resource != ImmaterialResourceManager.Resource.NoisePollution;

        public double GetFactor(ItemClass.Zone zone, ImmaterialResourceManager.Resource resource)
        {
            if (zone == ItemClass.Zone.Industrial)
            {
                switch (resource)
                {
                    case ImmaterialResourceManager.Resource.PublicTransport:
                        return 0.3333;
                    case ImmaterialResourceManager.Resource.PoliceDepartment:
                    case ImmaterialResourceManager.Resource.HealthCare:
                    case ImmaterialResourceManager.Resource.DeathCare:
                        return 0.2;
                    case ImmaterialResourceManager.Resource.FireDepartment:
                        return 0.5;
                    case ImmaterialResourceManager.Resource.Entertainment:
                    case ImmaterialResourceManager.Resource.EducationElementary:
                    case ImmaterialResourceManager.Resource.EducationHighSchool:
                    case ImmaterialResourceManager.Resource.EducationUniversity:
                        return 0.125;
                    case ImmaterialResourceManager.Resource.CargoTransport:
                        return 1;
                    case ImmaterialResourceManager.Resource.NoisePollution:
                    case ImmaterialResourceManager.Resource.Abandonment:
                        return -0.1429;
                    
                }
            }
            else if (zone == ItemClass.Zone.Office)
            {
                switch (resource)
                {
                    case ImmaterialResourceManager.Resource.PublicTransport:
                        return 0.3333;
                    case ImmaterialResourceManager.Resource.PoliceDepartment:
                    case ImmaterialResourceManager.Resource.HealthCare:
                    case ImmaterialResourceManager.Resource.DeathCare:
                    case ImmaterialResourceManager.Resource.FireDepartment:
                        return 0.2;
                    case ImmaterialResourceManager.Resource.Entertainment:
                        return 0.1667;
                    case ImmaterialResourceManager.Resource.EducationElementary:
                    case ImmaterialResourceManager.Resource.EducationHighSchool:
                    case ImmaterialResourceManager.Resource.EducationUniversity:
                        return 0.1429;
                    case ImmaterialResourceManager.Resource.NoisePollution:
                        return -0.25;
                    case ImmaterialResourceManager.Resource.Abandonment:
                        return -0.3333;
                }
            }
            else
            {
                switch (resource)
                {
                    case ImmaterialResourceManager.Resource.EducationElementary:
                    case ImmaterialResourceManager.Resource.EducationHighSchool:
                    case ImmaterialResourceManager.Resource.EducationUniversity:
                    case ImmaterialResourceManager.Resource.HealthCare:
                    case ImmaterialResourceManager.Resource.FireDepartment:
                    case ImmaterialResourceManager.Resource.PoliceDepartment:
                    case ImmaterialResourceManager.Resource.PublicTransport:
                    case ImmaterialResourceManager.Resource.DeathCare:
                    case ImmaterialResourceManager.Resource.Entertainment:
                    case ImmaterialResourceManager.Resource.CargoTransport:
                        return 1;
                    case ImmaterialResourceManager.Resource.NoisePollution:
                    case ImmaterialResourceManager.Resource.CrimeRate:
                    case ImmaterialResourceManager.Resource.FireHazard:
                    case ImmaterialResourceManager.Resource.Abandonment:
                        return -1;
                }
            }
            return 0;
        }

        public int GetPollutionScore(Building data, ItemClass.Zone zone)
        {
            Singleton<NaturalResourceManager>.instance.CheckPollution(data.m_position, out byte resourceRate13);
            return ImmaterialResourceManager.CalculateResourceEffect(resourceRate13, 50, 255, 50, 100);
        }

        public double GetServiceScore(int resourceRate, ImmaterialResourceManager.Resource resource, ItemClass.Zone zone,ref int maxLimit)
        {
            if (zone == ItemClass.Zone.ResidentialHigh || zone == ItemClass.Zone.ResidentialLow || zone == ItemClass.Zone.CommercialHigh || zone == ItemClass.Zone.CommercialLow)
                switch (resource)
                {
                    case ImmaterialResourceManager.Resource.NoisePollution:
                    case ImmaterialResourceManager.Resource.CrimeRate:
                        maxLimit = 100;
                        return ImmaterialResourceManager.CalculateResourceEffect(resourceRate, 10, 100, 0, 100);
                    case ImmaterialResourceManager.Resource.FireHazard:
                        maxLimit = 100;
                        return ImmaterialResourceManager.CalculateResourceEffect(resourceRate, 50, 100, 10, 50);
                    case ImmaterialResourceManager.Resource.Abandonment:
                        maxLimit = 50;
                        return ImmaterialResourceManager.CalculateResourceEffect(resourceRate, 15, 50, 100, 200);
                }

            maxLimit = 500;
            return ImmaterialResourceManager.CalculateResourceEffect(resourceRate, 100, 500, 50, 100);
        }

        public double GetServiceScore(ImmaterialResourceManager.Resource resource, ItemClass.Zone zone, ushort[] array, int num, ref int rawValue, ref int maxLimit)
            => GetServiceScore(array[num + (int)resource], resource, zone, ref maxLimit);

        public int GetProperServiceScore(ushort buildingID)
        {
            Building data = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID];
            Singleton<ImmaterialResourceManager>.instance.CheckLocalResources(data.m_position, out ushort[] array, out int num);
            double num2 = 0;
            var zone = data.Info.m_class.GetZone();
            for (var i = 0; i < 20; i += 1)
            {
                int max = 0;
                int raw = 0;
                var imr = (ImmaterialResourceManager.Resource)i;
                num2 += GetServiceScore(imr, zone, array, num, ref raw, ref max) * GetFactor(zone, imr);
            }

            num2 -= GetPollutionScore(data, zone) * GetPollutionFactor(zone);

            return Math.Max(0, (int)num2);
        }

        public void GetEducationHappyScore(ushort buildingID, out float education, out float happy, out float commute)
        {
            Citizen.BehaviourData behaviour = default(Citizen.BehaviourData);
            Building data = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID];
            ItemClass.Zone zone = data.Info.m_class.GetZone();

            commute = 0;

            int alive = 0;
            int total = 0;
            int COMPANYCount = 0;
            int aliveCOMPANYCount = 0;
            int emptyCOMPANY = 0;

            if (zone == ItemClass.Zone.ResidentialLow || zone == ItemClass.Zone.ResidentialHigh)
            {
                CitizenHelper.instance.GetHomeBehaviour(buildingID, data, ref behaviour, ref alive, ref total, ref COMPANYCount, ref aliveCOMPANYCount, ref emptyCOMPANY);
                if (alive > 0)
                {
                    int num = behaviour.m_educated1Count + behaviour.m_educated2Count * 2 + behaviour.m_educated3Count * 3;
                    int num2 = behaviour.m_teenCount + behaviour.m_youngCount * 2 + behaviour.m_adultCount * 3 + behaviour.m_seniorCount * 3;                    
                    if (num2 != 0)
                    {
                        education = (num * 72 + (num2 >> 1)) / num2;
                        happy =  behaviour.m_wellbeingAccumulation / (float)alive;
                        return;
                    }
                }
            }
            else if (zone == ItemClass.Zone.CommercialHigh || zone == ItemClass.Zone.CommercialLow)
            {
                CitizenHelper.instance.GetVisitBehaviour(buildingID, data, ref behaviour, ref alive, ref total);
                if (alive > 0)
                {
                    int num = num = behaviour.m_wealth1Count + behaviour.m_wealth2Count * 2 + behaviour.m_wealth3Count * 3;
                    education = (num * 18 + (alive >> 1)) / alive;
                    happy =  behaviour.m_wellbeingAccumulation / (float)alive;
                    commute = 0;
                    return;
                }
            }
            else if (zone == ItemClass.Zone.Office)
            {
                CitizenHelper.instance.GetWorkBehaviour(buildingID, data, ref behaviour, ref alive, ref total);
                int num = behaviour.m_educated1Count + behaviour.m_educated2Count * 2 + behaviour.m_educated3Count * 3;
                if (alive > 0)
                {
                    education = (num * 12 + (alive >> 1)) / alive;
                    happy = behaviour.m_wellbeingAccumulation / (float)alive;
                    return;
                }
            }
            else
            {
                CitizenHelper.instance.GetWorkBehaviour(buildingID, data, ref behaviour, ref alive, ref total);
                int num = behaviour.m_educated1Count + behaviour.m_educated2Count * 2 + behaviour.m_educated3Count * 3;
                if (alive > 0)
                {
                    education = num = (num * 20 + (alive >> 1)) / alive;
                    happy = behaviour.m_wellbeingAccumulation / (float)alive;
                    return;
                }
            }

            education = 0;
            happy = 0;
            commute = 0;
        }
        
        public int GetServiceThreshhold(ItemClass.Level level, ItemClass.Zone zone)
        {
            if (level == ItemClass.Level.None)
                return 0;

            ItemClass.Level maxLevel = ItemClass.Level.Level5;
            int multiplier = 0, start = 0;
            switch (zone)
            {
                case ItemClass.Zone.ResidentialLow:
                case ItemClass.Zone.ResidentialHigh:
                    multiplier = 15;
                    break;
                case ItemClass.Zone.CommercialLow:
                case ItemClass.Zone.CommercialHigh:
                    start = 1;
                    multiplier = 20;
                    break;
                case ItemClass.Zone.Industrial:
                    multiplier = 30;
                    break;
                case ItemClass.Zone.Office:
                    multiplier = 45;
                    break;
            }
            if (level == maxLevel)
                return int.MaxValue;

            return ((int)level + 1) * multiplier + start;
        }

        public int GetEducationThreshhold(ItemClass.Level level, ItemClass.Zone zone)
        {
            if (level == ItemClass.Level.None)
                return 0;

            ItemClass.Level maxLevel = ItemClass.Level.Level5;
            int start = 0;
            switch (zone)
            {
                case ItemClass.Zone.CommercialLow:
                case ItemClass.Zone.CommercialHigh:
                    start = 15;
                    break;
            }
            if (level == maxLevel)
                return int.MaxValue;

            return ((int)level + 1) * 15 + start;
        }
    }
}
