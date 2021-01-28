using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that allows cycling through assigned lists of type IManeuverableListEntry. can be assigned multiple lists of the same length
/// </summary>
public class CyclingLists
{
    //creating a list of lists, to affect multiple elements at once if they have the same length
    public List<List<IManeuverableListEntry>> affectedLists;

    /// <summary>
    /// the currently marked/selected entry
    /// </summary>
    int mE;
    /// <summary>
    /// Handles calls of marking and unmarking entries. removes all marks when below 0
    /// </summary>
    int MarkedEntry
    {
        get { return mE; }
        set
        {
            if (selected) return;

            if (value < 0)
            {
                unmarkEntry(mE);
                mE = - 1;           //no entry marked
                return;
            }

            unmarkEntry(mE);
            markEntry(value);
            mE = value;
        }
    }
    public int Index { get { if (mE < 0) return 0; return mE; } }

    bool selected;

    public CyclingLists(List<IManeuverableListEntry> firstList)
    {
        affectedLists = new List<List<IManeuverableListEntry>>();
        affectedLists.Add(firstList);

        //Mark no entry
        MarkedEntry = -1;
    }

    public void AddList(List<IManeuverableListEntry> list)
    {
        if (list.Count != affectedLists[0].Count)
        {
            Debug.LogWarning($"Added list does not have the same amount of entries as the base list: \nBase list has {affectedLists[0].Count}, while added list has {list.Count} entries");
            return;
        }

        affectedLists.Add(list);
    }

    #region public move and select functions
    public void moveSteps(int steps)
    {
        if (selected) return;

        if (MarkedEntry < 0)                        //when no entry was marked we start at the first entry
        {
            MarkedEntry = 0;
            return;
        }

        int i = MarkedEntry;

        i += steps;                                 //adding the assigned steps
        i = getIntInList(i);

        MarkedEntry = i;                            //marking the new entry
    }

    public void moveTo(int index)
    {
        index = getIntInList(index);

        MarkedEntry = index;
    }

    public void jumpOfList()
    {
        if (selected) return;
        MarkedEntry = -1;                           //Unmarks every entry on the list
    }

    public void selectCurrentEntry()
    {
        if (MarkedEntry < 0) return;    //no entry marked

        selectEntry(MarkedEntry);
        selected = true;
    }

    public void deselectCurrentEntry()
    {
        if (MarkedEntry < 0) return;    //no entry marked

        deselectEntry(MarkedEntry);
        selected = false;
    }
    #endregion

    #region privat select and mark funktions
    void selectEntry(int entry)
    {
        if (MarkedEntry < 0) return;    //no entry marked

        foreach (var list in affectedLists)
        {
            list[entry].SelectEntry();
        }
    }

    void deselectEntry(int entry)
    {
        if (MarkedEntry < 0) return;    //no entry marked

        foreach (var list in affectedLists)
        {
            list[entry].DeselectEntry();
        }
    }

    void markEntry(int entry)
    {
        if (MarkedEntry < 0) return;    //no entry marked

        foreach (var list in affectedLists)
        {
            list[entry].MarkEntry();
        }
    }

    void unmarkEntry(int entry)
    {
        if (MarkedEntry < 0) return;    //no entry marked

        foreach (var list in affectedLists)
        {
            list[entry].UnmarkEntry();
        }
    }

    /// <summary>
    /// returning a number within the base list range. jumping to the end when below 0 and to the start when above the maximum
    /// </summary>
    /// <param name="i">Any number</param>
    /// <returns>A number between 0 and the length of the base list</returns>
    int getIntInList(int i)
    {
        do
        {
            i += affectedLists[0].Count;                //adding the length of the list in case we were below zero, effectively jumping to the end of the list
            i %= affectedLists[0].Count;                //returning to start of list when passing the last entry
        } while (i < 0);

        return i;
    }
    #endregion
}

public interface IManeuverableListEntry
{
    void SelectEntry();
    void DeselectEntry();
    void MarkEntry();
    void UnmarkEntry();
}
