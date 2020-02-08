using Don_PlcDashboard_and_Reports.Data;
using Don_PlcDashboard_and_Reports.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Don_PlcDashboard_and_Reports.Services
{
    public class PlcService
    {
        public int Valoare { get; set; } = 1;// Valoare de proba comunicatie Blazor
        // List of Plcs
        public List<PlcModel> ListPlcs;
        // List of Tags
        public List<TagModel> ListTags;
        // Logger
        private readonly ILogger<PlcService> _logger;

        // Constructor
        public PlcService(ILogger<PlcService> logger)
        {
            ListPlcs = new List<PlcModel>();
            ListTags = new List<TagModel>();
            _logger = logger;
            _logger.LogInformation("{data}<=>{Messege}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), "A pornit PlcService din PlcService Constructor");
        }

        // Get New Plc object by PlcModel
        public Plc GetNewPlcFromPlcModel(PlcModel plcModel)
        {
            return new Plc(plcModel.CpuType, plcModel.Ip, plcModel.Rack, plcModel.Slot);
        }

        // Connect Plc
        public void ConnectPlc(Plc plc)
        {
            try
            {
                var _cancelTasks = new CancellationTokenSource();
                var task = Task.Run(() =>
                {
                    if (!plc.IsConnected)
                    {
                        try
                        {
                            plc.Open(); // Deschidere conexiune Plc
                        }
                        catch (PlcException exPlc)
                        {

                            _logger.LogError("{data} {exMessege} IP: {ip}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), exPlc.Message, plc.IP);
                        }
                        _logger.LogInformation("{data}<=>{Messege} IP: {ip}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), "S-a deschis conexiune cu plc", plc.IP);
                    }
                    else return;
                }, _cancelTasks.Token);
                if (!task.Wait(TimeSpan.FromSeconds(0.5))) _cancelTasks.Cancel(); // Daca nu mai raspunde in timp util se opreste Task
            }
            catch (PlcException exPlc)
            {
                _logger.LogError("{data} {exMessege} IP: {ip}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), exPlc.Message, plc.IP);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError("{data} {exMessege} IP: {ip}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), ex.Message, plc.IP);
            }
            catch (Exception ex)
            {
                _logger.LogError("{data} {exMessege} IP: {ip}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), ex.Message, plc.IP);
            }
        }

        // Disconnect Plc
        public void DisconnectPlc(Plc plc)
        {
            try
            {
                var _cancelTasks = new CancellationTokenSource();
                var task = Task.Run(() =>
                {
                    if (plc.IsConnected)
                    {
                        plc.Close(); // Inchidere conexiune Plc
                        _logger.LogInformation("{data}<=>{Messege} IP: {ip}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), "S-a inchis conexiune cu plc", plc.IP);
                    }
                    else return;
                }, _cancelTasks.Token);
                if (!task.Wait(TimeSpan.FromSeconds(0.5))) _cancelTasks.Cancel(); // Daca nu mai raspunde in timp util se opreste Task
            }
            catch (PlcException exPlc)
            {
                _logger.LogError("{data} {exMessege} IP: {ip}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), exPlc.Message, plc.IP);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError("{data} {exMessege} IP: {ip}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), ex.Message, plc.IP);
            }
            catch (Exception ex)
            {
                _logger.LogError("{data} {exMessege} IP: {ip}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), ex.Message, plc.IP);
            }
        }

        // Initiate List of Plcs from DbContext
        public async Task InitializeListOfPlcAsync(RaportareDbContext context)
        {
            ListPlcs.Clear();
            IEnumerable<PlcModel> listPlcs = await context.Plcs.ToListAsync();
            IEnumerable<TagModel> tagList = await context.Tags.ToListAsync();
            // Add list of Plcs(with Plc object and TagList) from DbContext To PlcService PLCList
            foreach (var plcModel in listPlcs)
            {
                plcModel.PlcObject = GetNewPlcFromPlcModel(plcModel); // Create Plc object
                plcModel.TagsList = tagList.Where(t => t.PlcModelID == plcModel.PlcModelID).ToList();
                plcModel.PingRequestsFail = 0; // Initiate with 0 ping requests fail
                ListPlcs.Add(plcModel);
            }
        }

        // Read Tag value
        public void GetTagValueFromPlc(TagModel tag)
        {
            try
            {
                switch (tag.DataType)
                {
                    case VarType.Bit:
                        tag.Value = Convert.ToBoolean(tag.PlcModel.PlcObject.Read(tag.Adress)).ToString();
                        break;
                    case VarType.Byte:
                        break;
                    case VarType.Word:
                        break;
                    case VarType.DWord:
                        break;
                    case VarType.Int:
                        tag.Value = Convert.ToInt16(tag.PlcModel.PlcObject.Read(tag.Adress)).ToString();
                        break;
                    case VarType.DInt:
                        tag.Value = Convert.ToInt32(tag.PlcModel.PlcObject.Read(tag.Adress)).ToString();
                        break;
                    case VarType.Real:
                        tag.Value = Convert.ToDouble(tag.PlcModel.PlcObject.Read(tag.Adress)).ToString();
                        break;
                    case VarType.String:
                        break;
                    case VarType.StringEx:
                        break;
                    case VarType.Timer:
                        break;
                    case VarType.Counter:
                        break;
                    case VarType.DateTime:
                        break;
                    default:
                        break;
                }
            }
            catch (PlcException plcEx)
            {
                _logger.LogInformation("{data}<=>{Messege} {error}. PlcName: {PlcName}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), "Error reading tag value from plc: ", plcEx.Message, tag.PlcModel.Name);
            }
        }

        // Check pingRequests grater than a nr and disable Plc
        public void IsPingRequestsFailGrateThan(PlcModel plc, int nr)
        {
            if (plc.PingRequestsFail >= nr) plc.IsEnable = false;
        }

        // Reset pingRequests Fail at a certain time
        public void RemakeEnablePlc(PlcModel plc)
        {
            if (DateTime.Now.Second == 10)
            {
                plc.PingRequestsFail = 0;
                plc.IsEnable = true;
            }
        }

        // Try to reconnect Plc in automatic mode if it is available
        // Disable Plc if PingRequestsFail => nr
        // return true if it is connected
        public bool IsConnectedPlc(PlcModel plc)
        {
            // If Plc RequestsFail is grater than a nr, disable plc
            IsPingRequestsFailGrateThan(plc, 5);
            // Try to reconnect plc if it is available
            try
            {
                if (IsAvailableIpAdress(plc))
                {
                    // Check if it is connected
                    if (!plc.PlcObject.IsConnected) plc.PlcObject.Open(); // if it is not connected => reconnect
                    if (plc.PlcObject.IsConnected) return true; // if is connected return true
                    return false;
                }
                else return false;
            }
            catch (PlcException exPlc)
            {

                _logger.LogError("{data} {exMessege} IP: {ip}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), exPlc.Message, plc.Ip);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError("{data} {exMessege} IP: {ip}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), ex.Message, plc.Ip);
                return false;
            }
        }


        // Update Tag List Values of Plc
        public void RefreshTagValues(PlcModel plc)
        {
            // Read Tag values from Plc
            foreach (TagModel tag in plc.TagsList)
            {
                try
                {
                    var _cancelTasks = new CancellationTokenSource();
                    var task = Task.Run(() =>
                    {
                        if (plc.PlcObject.IsConnected)
                        {
                            //_lastScanTime = DateTime.Now;
                            GetTagValueFromPlc(tag); // Citire semnale Plc
                            //ScanTime = DateTime.Now - _lastScanTime; // Determinare ScanTime Plc
                        }
                        else
                        {
                            return;
                        };
                    }, _cancelTasks.Token);
                    if (!task.Wait(TimeSpan.FromSeconds(0.2))) _cancelTasks.Cancel(); // Daca nu mai raspunde in timp util se opreste Task
                }
                catch (PlcException exPlc)
                {
                    _logger.LogError("{data} {exMessege} IP: {ip}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), exPlc.Message, plc.Name);
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogError("{data} {exMessege} IP: {ip}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), ex.Message, plc.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError("{data} {exMessege} IP: {ip}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), ex.Message, plc.Name);
                }
            }
        }

        // Functie Verificare Adresa IP
        public bool IsAvailableIpAdress(PlcModel plc)
        {
            Ping ping = new Ping();
            try
            {
                PingReply reply = ping.Send(plc.Ip, 200);
                if (reply.Status.ToString() == "Success")
                {
                    plc.PingRequestsFail = 0;
                    return true;
                }
                plc.PingRequestsFail++;
                return false;
            }
            catch(System.Net.NetworkInformation.PingException exPing)
            {
                _logger.LogError("{data} {exMessege} PlcName: {name}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), exPing.Message, plc.Name);
                plc.PingRequestsFail++;
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError("{data} {exMessege} PlcName: {name}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), ex.Message, plc.Name);
                plc.PingRequestsFail++;
                return false;
            }
        }

        // Update Database Context Tag Values
        public Task UpdateDbContextTagsValue(RaportareDbContext context, List<TagModel> tags)
        {
            foreach (TagModel tag in tags)
            {
                try
                {
                    context.Update(tag); // update tag value in DbContext
                }
                catch (InvalidOperationException exEFUpdate)
                {

                    _logger.LogError("{data} {exMessege} PlcName: {name}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), exEFUpdate.Message, tag.PlcModel.Name);
                }
            }
            return Task.CompletedTask;
        }

        // Check if a plc name is already in listPlcs
        public bool IsPlcNameInListPlcs(string plcName)
        {
            if (plcName == null)
            {
                return false;
            }
            var plcModel = ListPlcs.FirstOrDefault(m => m.Name == plcName);
            if (plcModel == null)
            {
                return false;
            }
            _logger.LogInformation("{data}<=>{Messege} PlcName:{name}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), "S-a gasit PlcModel in ListPlcs with name", plcName);
            return true;
        }

        public string ClickTestare()
        {
            _logger.LogInformation("{data} {exMessege}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), "Click din View component");
            return "A fost apasat click din View";
        }

    }
}
