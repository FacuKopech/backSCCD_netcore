using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Model.Entities;

namespace SCCD.Controllers
{
    [ApiController]
    public class SistemaController : Controller
    {
        [HttpGet]
        [Route("/[controller]/[action]/")]
        public IActionResult Backup()
        {            
            try
            {
                var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

                string serverName = config["ServerName"];
                string databaseName = config["DatabaseName"];

                ServerConnection serverConnection = new ServerConnection(serverName);
                serverConnection.EncryptConnection = true;
                serverConnection.TrustServerCertificate = true;
                Server sqlServer = new Server(serverConnection);

                Backup backup = new Backup
                {
                    Action = BackupActionType.Database,
                    Database = databaseName
                };
                
                string backupFilePath = @"C:\Users\facuk\DBBackups\DBBackup.bak";
                backup.Devices.AddDevice(backupFilePath, DeviceType.File);

                backup.SqlBackup(sqlServer);
                BackupFilePath backupObj = new BackupFilePath { 
                    path = backupFilePath
                };
                return Ok(backupObj);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("/[controller]/[action]/")]
        public IActionResult Restore()
        {
            try
            {
                var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

                string serverName = config["ServerName"];
                string databaseName = config["DatabaseName"];

                ServerConnection serverConnection = new ServerConnection(serverName);
                serverConnection.EncryptConnection = true;
                serverConnection.TrustServerCertificate = true;
                Server sqlServer = new Server(serverConnection);

                string setSingleUserQuery = $"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
                sqlServer.ConnectionContext.ExecuteNonQuery(setSingleUserQuery);

                Restore restore = new Restore
                {
                    Action = RestoreActionType.Database,
                    Database = databaseName                    
                };

                string backupFilePath = @"C:\Users\facuk\DBBackups\DBBackup.bak";
                restore.Devices.AddDevice(backupFilePath, DeviceType.File);

                string dataFilePath = sqlServer.Databases[databaseName].FileGroups[0].Files[0].FileName;
                string logFilePath = sqlServer.Databases[databaseName].LogFiles[0].FileName;

                restore.RelocateFiles.Add(new RelocateFile(databaseName, dataFilePath));
                restore.RelocateFiles.Add(new RelocateFile(databaseName + "_log", logFilePath));

                string restoreQuery = $"RESTORE DATABASE {databaseName} FROM DISK = {backupFilePath} WITH REPLACE";
                sqlServer.ConnectionContext.ExecuteNonQuery(setSingleUserQuery);


                string setMultiUserQuery = $"ALTER DATABASE [{databaseName}] SET MULTI_USER;";
                sqlServer.ConnectionContext.ExecuteNonQuery(setMultiUserQuery);

                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }

    public class BackupFilePath
    {
        public string path { get; set; }
    }
}
