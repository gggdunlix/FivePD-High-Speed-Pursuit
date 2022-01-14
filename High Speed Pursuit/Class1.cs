using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using FivePD.API;
using FivePD.API.Utils;



namespace High_Speed_Pursuit
{
    [CalloutProperties("High Speed Pursuit", "GGGDunlix", "0.1.2")]
    public class HighSpeedPursuit : Callout
    {
        Ped suspect;
        Vehicle car;

        public HighSpeedPursuit()
        {
            Random random = new Random();
            InitInfo(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(Vector3Extension.Around(Game.PlayerPed.Position, 200f))));
            ShortName = "High Speed Pursuit";
            CalloutDescription = "The suspect was spotted by traffic cameras after eluding police. Re-engage in the pursuit. Respond in Code 2.";
            ResponseCode = 2;
            StartDistance = 60f;
        }

        public async override Task OnAccept()
        {
            InitBlip();
            UpdateData();
            var carlist = new[]
            {
                VehicleHash.Adder,
                VehicleHash.Coquette,
                VehicleHash.Coquette2,
                VehicleHash.Coquette3,
                VehicleHash.Cheetah,
                VehicleHash.Cheetah2,
                VehicleHash.Comet2,
                VehicleHash.Comet3
            };
            car = await SpawnVehicle(carlist[RandomUtils.Random.Next(carlist.Length)], Location);
            suspect = await SpawnPed(RandomUtils.GetRandomPed(), Location);
            suspect.AlwaysKeepTask = true;
            suspect.BlockPermanentEvents = true;
        }

        public override void OnStart(Ped player)
        {
            base.OnStart(player);
            suspect.AttachBlip();
            Utilities.ExcludeVehicleFromTrafficStop(car.NetworkId, true);
            suspect.SetIntoVehicle(car, VehicleSeat.Driver);
            Pursuit.RegisterPursuit(suspect);
            suspect.Task.FleeFrom(Game.PlayerPed);
            Utilities.RequestBackup(Utilities.Backups.Code99);
            ShowNetworkedNotification("Pursuit engaged. Code 99 Backup requested.", "CHAR_CALL911", "CHAR_CALL911", "Dispatch", "Pursuit", 15f);
            suspect.DrivingSpeed = 230;
            suspect.DrivingStyle = DrivingStyle.Rushed;
            car.MaxSpeed = 330;

        }
    }


}
