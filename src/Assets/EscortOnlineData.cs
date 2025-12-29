using System;
using JetBrains.Annotations;
using RainMeadow;
using static TheEscort.Eshelp;

namespace TheEscort;

public class EscortOnlineData : OnlineEntity.EntityData
{
    [UsedImplicitly]
    public EscortOnlineData()
    {
    }
    public override EntityDataState MakeState(OnlineEntity entity, OnlineResource inResource)
    {
        return new State(entity);
    }
    public class State : EntityDataState
    {
        [OnlineField(nullable = true)]
        public int? buildId;

        [UsedImplicitly]
        public State()
        {
        }

        public State(OnlineEntity oe)
        {
            if ((oe as OnlinePhysicalObject)?.apo?.realizedObject is not Player player) return;
            if (!Plugin.eCon.TryGetValue(player, out Escort e)) return;
            buildId = FindBuildIndex(e);
            Ebug("Save value " + buildId, LogLevel.MESSAGE);
        }

        public override void ReadTo(OnlineEntity.EntityData data, OnlineEntity onlineEntity)
        {
            if ((onlineEntity as OnlinePhysicalObject)?.apo?.realizedObject is not Player player) return;
            Ebug("Return value " + buildId, LogLevel.MESSAGE);
            if (!Plugin.Esconfig_Build(player, buildId??1)) return;
        }

        public static int FindBuildIndex(in Escort e)
        {
            if (e.Brawler) return -1;
            if (e.Deflector) return -2;
            if (e.Escapist) return -3;  // Yeah not letting new Escapist work on Meadow, too janky to fix.
            if (e.Railgunner) return -4;
            if (e.Speedster) return -5;
            if (e.Gilded) return -6;
            if (e.Barbarian) return -7;
            if (e.Unstable) return -8;
            // if (e.) return -9;
            return 0;
        }

        public override Type GetDataType()
        {
            return typeof(EscortOnlineData);
        }
    }
}
