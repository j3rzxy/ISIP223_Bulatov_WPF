using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP223_Bulatov_WPF
{
    public static class CompatibilityChecker
    {
        public static string CheckCompatibility(List<PartSlot> slots)
        {
            int cpuId = slots.FirstOrDefault(s => s.CategoryId == 1)?.SelectedPartId ?? 0;
            int mbId = slots.FirstOrDefault(s => s.CategoryId == 4)?.SelectedPartId ?? 0;
            int ramId = slots.FirstOrDefault(s => s.CategoryId == 3)?.SelectedPartId ?? 0;
            int caseId = slots.FirstOrDefault(s => s.CategoryId == 5)?.SelectedPartId ?? 0;
            int psuId = slots.FirstOrDefault(s => s.CategoryId == 6)?.SelectedPartId ?? 0;
            int gpuId = slots.FirstOrDefault(s => s.CategoryId == 2)?.SelectedPartId ?? 0;
            int coolerId = slots.FirstOrDefault(s => s.CategoryId == 7)?.SelectedPartId ?? 0;

            var cpu = Core.Context.cpu_.FirstOrDefault(x => x.id == cpuId);
            var mb = Core.Context.motherboard_.FirstOrDefault(x => x.id == mbId);
            var ram = Core.Context.ram_.FirstOrDefault(x => x.id == ramId);
            var pcCase = Core.Context.case_.FirstOrDefault(x => x.id == caseId);
            var psu = Core.Context.powersupply_.FirstOrDefault(x => x.id == psuId);
            var gpu = Core.Context.gpu_.FirstOrDefault(x => x.id == gpuId);
            var cooler = Core.Context.processorcooler_.FirstOrDefault(x => x.id == coolerId);


            // Проверка Сокета (Процессор + Мат.плата)
            if (cpu != null && mb != null)
            {
                if (cpu.socketid != mb.socketid)
                    return $"Несовместимые сокеты: у CPU — {cpu.socket_.name}, у MB — {mb.socket_.name}.";
            }

            // Проверка Оперативной памяти (Тип DDR)
            if (mb != null && ram != null)
            {
                if (mb.memorytypeid != ram.memorytypeid)
                    return $"Разные типы памяти: плата поддерживает {mb.memorytype_.name}, а RAM — {ram.memorytype_.name}.";

                if (ram.count > mb.memoryslots)
                    return $"Недостаточно слотов RAM: на плате {mb.memoryslots}, выбрано планок {ram.count}.";
            }

            // Проверка Корпуса и Мат.платы (Форм-фактор)
            if (mb != null && pcCase != null)
            {
                bool canFit = Core.Context.boardformfactorcase_.Any(x => x.caseid == caseId && x.formfactorid == mb.formfactorid);
                if (!canFit)
                    return $"Корпус не поддерживает форм-фактор платы {mb.formfactor_.name}.";
            }

            // Проверка Сокета Кулера
            if (cpu != null && cooler != null)
            {
                bool hasBracket = Core.Context.socketprocessorcooler_.Any(x => x.processorcoolerid == coolerId && x.socketid == cpu.socketid);
                if (!hasBracket)
                    return $"Кулер не имеет крепления для сокета {cpu.socket_.name}.";
            }

            // Проверка Мощности БП
            if (psu != null)
            {
                int requiredPower = (cpu?.thermalpower ?? 0) + (gpu?.recommendpower ?? 0);
                if (psu.power < requiredPower)
                    return $"Малая мощность БП: нужно минимум {requiredPower}W, у выбранного — {psu.power}W.";
            }

            return "Совместимо";

        }
    }

}