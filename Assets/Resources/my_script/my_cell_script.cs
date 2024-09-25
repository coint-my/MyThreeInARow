using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MySortArray : IComparer<my_cell_script>
{
    public int Compare(my_cell_script _first, my_cell_script _second)
    {
        if (_first.myId > _second.myId)
            return 1;
        else if (_first.myId < _second.myId)
            return -1;
        else return 0;
    }
}

public class my_cell_script : MonoBehaviour
{
    public int myId;

    public my_active_cell_move MyGetActiveCell { get; private set; }

    private void Start()
    {
        StringBuilder testString = new StringBuilder();
        string oldName = transform.name;
        for (int ind = 0; ind < oldName.Length; ind++)
        {
            if (oldName[ind] >= '0' && oldName[ind] <= '9')
                testString.Append(oldName[ind]);
        }

        myId = int.Parse(testString.ToString());

        MyGetActiveCell = GetComponentInChildren<my_active_cell_move>();
    }

    public void MyInitialize()
    {
        MyGetActiveCell = GetComponentInChildren<my_active_cell_move>();
    }
}
