using ColossalFramework;
using System;

namespace ExtendedBuildings
{
    class CitizenHelper
    {
        private static CitizenHelper instance;
        public static CitizenHelper Instance
        {
            get
            {
                if (instance == null)
                    instance = new CitizenHelper();

                return instance;
            }
        }

        public void GetHomeBehaviour(Building building, ref Citizen.BehaviourData behaviour, ref int aliveCount, ref int totalCount, ref int COMPANYCount, ref int aliveCOMPANYCount, ref int emptyCOMPANYCount)
        {
            CitizenManager instance = Singleton<CitizenManager>.instance;
            uint num = building.m_citizenUnits;
            int num2 = 0;
            while (num != 0u)
            {
                if (EnumExtensions.IsFlagSet(instance.m_units.m_buffer[num].m_flags, CitizenUnit.Flags.Home))
                {
                    int num3 = 0;
                    int num4 = 0;
                    instance.m_units.m_buffer[num].GetCitizenHomeBehaviour(ref behaviour, ref num3, ref num4);
                    if (num3 != 0)
                    {
                        aliveCOMPANYCount++;
                        aliveCount += num3;
                    }
                    if (num4 != 0)
                        totalCount += num4;
                    else
                        emptyCOMPANYCount++;

                    COMPANYCount++;
                }
                num = instance.m_units.m_buffer[num].m_nextUnit;
                if (++num2 > 524288)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                    break;
                }
            }
        }

        public void GetWorkBehaviour(Building building, ref Citizen.BehaviourData behaviour, ref int aliveCount, ref int totalCount)
        {
            CitizenManager instance = Singleton<CitizenManager>.instance;
            uint num = building.m_citizenUnits;
            int num2 = 0;
            while (num != 0u)
            {
                if (EnumExtensions.IsFlagSet(instance.m_units.m_buffer[num].m_flags, CitizenUnit.Flags.Work))
                    instance.m_units.m_buffer[num].GetCitizenWorkBehaviour(ref behaviour, ref aliveCount, ref totalCount);

                num = instance.m_units.m_buffer[num].m_nextUnit;
                if (++num2 > 524288)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                    break;
                }
            }
        }

        public void GetVisitBehaviour(Building building, ref Citizen.BehaviourData behaviour, ref int aliveCount, ref int totalCount)
        {
            CitizenManager instance = Singleton<CitizenManager>.instance;
            uint num = building.m_citizenUnits;
            int num2 = 0;
            while (num != 0u)
            {
                if (EnumExtensions.IsFlagSet(instance.m_units.m_buffer[num].m_flags, CitizenUnit.Flags.Visit))
                    instance.m_units.m_buffer[num].GetCitizenVisitBehaviour(ref behaviour, ref aliveCount, ref totalCount);

                num = instance.m_units.m_buffer[num].m_nextUnit;
                if (++num2 > 524288)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                    break;
                }
            }
        }
    }
}
