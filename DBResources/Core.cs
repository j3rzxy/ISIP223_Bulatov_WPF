using ISIP223_Bulatov_WPF.DBResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP223_Bulatov_WPF
{
    internal class Core
    {
        public static PCBuilderDBEntities Context = new PCBuilderDBEntities();
        public static assembly CurrentAssembly;
        public static List<PartSlot> PartSlots;

        public static Dictionary<string, string> LoadSpecs(basepart part)
        {
            var specs = new Dictionary<string, string>();

            switch (part.parttypeid)
            {
                case 1: // CPU
                    var cpu = Core.Context.cpu_.Find(part.id);
                    if (cpu != null)
                    {
                        specs.Add("Сокет", cpu.socket_?.name ?? "н/д");
                        specs.Add("Ядра", cpu.numberofcores.ToString());
                        specs.Add("Базовая частота", cpu.basecorefrequency.ToString() + " MHz");
                        specs.Add("Максимальная частота", cpu.maxcorefrequency.ToString() + " MHz");
                        specs.Add("Кэш L3", cpu.cachel3.ToString() + " МБ");
                        specs.Add("Встроенный ГП", cpu.igpu_?.name?.ToString() ?? "нет");
                        specs.Add("Потребление", cpu.thermalpower.ToString() + " Вт");
                    }
                    break;

                case 2: // GPU
                    var gpu = Core.Context.gpu_.Find(part.id);
                    if (gpu != null)
                    {
                        var sb = new StringBuilder();
                        foreach (var c in gpu.videoconnectorgpu_)
                        {
                            sb.Append(c.videoconnector_.name.ToString());
                            sb.Append(", ");
                        }
                        specs.Add("Объем видео памяти", gpu.videomemory.ToString() + " ГБ");
                        specs.Add("Мин. БП", gpu.recommendpower.ToString() + " Вт");
                        specs.Add("Интерфейс", gpu.gpuinterface_?.name?.ToString() ?? "н/д");
                        specs.Add("Частота", gpu.chipfrequency.ToString() + " MHz");
                        specs.Add("Разъемы", sb.ToString());
                    }
                    break;

                case 3: // Ram
                    var ram = Core.Context.ram_.Find(part.id);
                    if (ram != null)
                    {
                        specs.Add("Тип", ram.memorytype_?.name ?? "н/д");
                        specs.Add("Объем", ram.capacity.ToString() + " ГБ");
                        specs.Add("Кол-во", ram.count.ToString());
                        specs.Add("Частота", ram.ghz.ToString() + " GHz");
                        specs.Add("Тайминги", ram.timings.ToString());
                    }
                    break;

                case 4: // Mother
                    var mb = Core.Context.motherboard_.Find(part.id);
                    if (mb != null)
                    {
                        specs.Add("Сокет", mb.socket_?.name ?? "н/д");
                        specs.Add("Формфактор", mb.formfactor_?.name ?? "н/д");
                        specs.Add("Слотов памяти", mb.memoryslots.ToString());
                        specs.Add("Тип памяти", mb.memorytype_?.name?.ToString() ?? "н/д");
                        specs.Add("Слотов PCI", mb.pcislots.ToString());
                        specs.Add("Слотов SATA", mb.sataports.ToString());
                        specs.Add("Разъемов USB", mb.usbports.ToString());
                    }
                    break;

                case 5: // Case
                    var cs = Core.Context.case_.Find(part.id);
                    if (cs != null)
                    {
                        specs.Add("Размер", cs.casesize_?.name ?? "н/д");
                        specs.Add("Слоты расширения", cs.expansionslots.ToString());
                        specs.Add("Вентиляторы", cs.fans.ToString());
                    }
                    break;

                case 6: // Power
                    var pw = Core.Context.powersupply_.Find(part.id);
                    if (pw != null)
                    {
                        specs.Add("Мощность", pw.power.ToString() + " Вт");
                        specs.Add("Габариты", pw.fandimension_?.name?.ToString() ?? "н/д");
                        specs.Add("Сертификация", pw.certificate_?.name?.ToString() ?? "н/д");
                    }
                    break;

                case 7: // Cooler
                    var pc = Core.Context.processorcooler_.Find(part.id);
                    if (pc != null)
                    {
                        specs.Add("Габариты", pc.fandimension_?.name?.ToString() ?? "н/д");
                        specs.Add("Тепловые трубки", pc.heatpipes.ToString());
                        specs.Add("Мин скорость", pc.minspeed.ToString() + " об/мин");
                        specs.Add("Макс скорость", pc.maxspeed.ToString() + " об/мин");
                        specs.Add("Уровень шума", pc.noiselevel.ToString() + " дБ");
                    }
                    break;

                case 8: // Storage
                    var st = Core.Context.storagedevice_.Find(part.id);
                    if (st != null)
                    {
                        specs.Add("Объем", st.capacity.ToString() + " ГБ");
                        specs.Add("Интерфейс", st.storagedeviceinterface_?.name?.ToString() ?? "н/д");
                        specs.Add("Тип", st.storagedevicetype_?.name?.ToString() ?? "н/д");
                    }
                    var hdd = Core.Context.hdd_.Find(part.id);
                    if (hdd != null)
                    {
                        specs.Add("Скорость врщения", hdd.rotationspeed.ToString() + " об/мин");
                    }
                    var ssd = Core.Context.ssd_.Find(part.id);
                    if (ssd != null)
                    {
                        specs.Add("TBW ", ssd.tbw.ToString() + " ТБ");
                    }
                    break;
            }
            return specs;
        }
    }
    public class PartSlot
    {
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public string SelectedPartName { get; set; } = "Не выбрано";
        public int SelectedPartId { get; set; }
        public decimal Price { get; set; } = 0;
        public string ImagePath { get; set; }
        public string PriceString => Price > 0 ? $"{Price:N2} ₽" : "—";
    }
}
