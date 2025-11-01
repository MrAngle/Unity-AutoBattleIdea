// Assets/Scripts/Combat/Flow/RoutingContracts.cs

using Combat.Flow.Domain;
using Combat.Flow.Domain.Aggregate;
using Inventory.Items.Domain;
using UnityEngine;

namespace Combat.Flow.Domain.Router
{
    // public readonly struct RouteDecision
    // {
    //     public IFlowNode NextNode { get; }
    //     public Vector2Int EntryCell { get; } // kratka, przez którą wchodzimy do kolejnego itemu
    //
    //     public RouteDecision(IFlowNode nextNode, Vector2Int entryCell)
    //     {
    //         NextNode = nextNode;
    //         EntryCell = entryCell;
    //     }
    // }

    public interface IFlowRouter
    {
        /// Zwraca null — jeśli brak kandydata (koniec).
        IPlacedItem DecideNext(IPlacedItem current, FlowModel model, System.Collections.Generic.IReadOnlyCollection<long> visitedNodeIds);
    }
}


/*
-----------------------------------
 FLOW ROUTER - DEFINICJA DZIAŁANIA
-----------------------------------

1. Inicjalizacja:
   - Router nie wie, kto go rozpoczął (np. damage, mana itp.).
   - Starter (np. input damage) uruchamia proces i określa punkt startowy w siatce (np. cell 0,0).
   - Router sprawdza, czy w danym punkcie (cell) znajduje się przedmiot.
     → Jeśli brak przedmiotu – flow kończy się.

2. Przetwarzanie bieżącego przedmiotu:
   - Jeśli przedmiot istnieje, jest procesowany (logika efektu jest poza routerem).
   - Router analizuje kształt przedmiotu (ItemShape), który może zajmować wiele pól (np. L, kwadrat, linia itp.).

3. Wyszukiwanie sąsiadów:
   - Router sprawdza wszystkie kratki sąsiadujące z zajmowanym obszarem przedmiotu.
     • Dla przedmiotu 1x1 → 4 pola (góra, dół, lewo, prawo).
     • Dla większego (np. 2x2) → do 8 pól (po 2 w każdym kierunku).
   - Sąsiad jest kandydatem, jeśli:
       → jego kratka ma stan `Occupied`
       → i nie był już uwzględniony w poprzednich krokach flow.
   - Router ignoruje kratki i przedmioty, które już brały udział w przepływie.

4. Wybór następnego przedmiotu:
   - Jeśli znaleziono wielu kandydatów → router losuje jednego z nich.
   - Router zapisuje informację, która kratka została wybrana (debug / analiza).

5. Brak kandydatów:
   - Jeśli nie znaleziono żadnych sąsiadów – router zwraca `null`.
   - Obsługę zakończenia flow przejmuje wyższy poziom (np. agregat).

-----------------------------------
W skrócie:
Router przeszukuje sąsiadujące kratki wokół aktywnego przedmiotu,
pomija już odwiedzone pozycje, losowo wybiera kolejny element i
zwraca informację o przejściu lub null, gdy flow się kończy.
-----------------------------------
*/
