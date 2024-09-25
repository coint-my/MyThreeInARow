using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public enum MyDirectionCell { LEFT, MIDDLE, RIGHT }

public class MySortList : IComparer<my_active_cell_move>
{
    public int Compare(my_active_cell_move _first, my_active_cell_move _second)
    {
        if (_first && _second)
        {
            if (_first.MyId > _second.MyId)
                return 1;
            else if (_first.MyId < _second.MyId)
                return -1;
            else return 0;
        }
        else
            return -10;
    }

    public static int MyStringToInt(string _name)
    {
        StringBuilder testString = new StringBuilder();
        string oldName = _name;

        for (int ind = 0; ind < oldName.Length; ind++)
        {
            if (oldName[ind] >= '0' && oldName[ind] <= '9')
                testString.Append(oldName[ind]);
        }

        if (testString.Length == 0)
        {
            Debug.Log("sorted exeption method MyStringToInt string = " + _name);
            return -1;
        }

        return int.Parse(testString.ToString());
    }
}
public class MySortListCollider2D : IComparer<Collider2D>
{
    public int Compare(Collider2D _first, Collider2D _second)
    {
        if (_first && _second)
        {
            int f = MySortList.MyStringToInt(_first.gameObject.transform.name);
            int s = MySortList.MyStringToInt(_second.gameObject.transform.name);

            if (f > s)
                return 1;
            else if (f < s)
                return -1;
            else return 0;
        }
        else
            return -10;
    }
}
public class MySortListGemCount : IComparer<MyPairTypeCount>
{
    public int Compare(MyPairTypeCount _first, MyPairTypeCount _second)
    {
        if (_first.myCount > _second.myCount)
            return 1;
        else if (_first.myCount < _second.myCount)
            return -1;
        else return 0;
    }
}

public class my_active_cell_move : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private int my_id;

    public my_active_cell_move[] myActiveCellMoveCollider;
    public List<my_active_cell_move> myListMoveColliders;//to do this

    private Vector2 myOldPositionMouse;
    private Vector2 myPositionMouse;

    private const float myTimeStop = 40f;
    private static float myTimeCurrent = 0;

    public my_gem myGem;

    public bool MyIsBusyCell;
    public bool MyIsTestBusy = false;
    public bool MyIsSelectedAnimate = false;

    public int MyId { get { return my_id; } }

    public bool MyIsHaveGem { get { return transform.childCount > 0; } }

    public my_gem MyGetChildrenGem { get { return transform.GetComponentInChildren<my_gem>(); } }

    public static event System.Action<my_active_cell_move, MyDirection> MyEventMouseClickOnCell;

    private void Awake()
    {
        my_id = MySortList.MyStringToInt(transform.parent.name);

        myListMoveColliders = new List<my_active_cell_move>();
    }

    public void MyInitializeColliders()
    {
        Vector3 offset = new Vector3(0, -100, 0);
        Collider2D[] col = Physics2D.OverlapBoxAll(transform.position + offset, new Vector2(300, 100), 0);
        myActiveCellMoveCollider = new my_active_cell_move[3] { null, null, null};

        MySortListCollider2D mySort = new MySortListCollider2D();
        Array.Sort(col, mySort);

        for (int ind = 0; ind < col.Length; ind++)
        {
            if (col[ind].GetComponentInChildren<my_active_cell_move>())
                myActiveCellMoveCollider[ind] = col[ind].GetComponentInChildren<my_active_cell_move>();
        }
    }

    public my_active_cell_move MyCheckAnimationDirection(MyDirectionCell _directionCell)
    {
        return MyCheckCorrectCell(this, (int)_directionCell);
    }

    public my_active_cell_move MyAddAnimationDown(my_active_cell_move _source)
    {
        //print("MyAddAnimationDown");
        my_active_cell_move helper = this;
        my_active_cell_move lastHelper = null;

        while (helper)
        {
            helper = MyCheckCorrectCell(helper, (int)MyDirectionCell.MIDDLE);

            if (helper != null)
            {
                MyIsTestBusy = false;

                lastHelper = helper;
                _source.myListMoveColliders.Add(lastHelper);
            }
        }

        if (lastHelper != null)
        {
            lastHelper.MyIsTestBusy = true;
            //print("end MyAddAnimationDown");
            return lastHelper;
        }
        //print("end null MyAddAnimationDown");
        return null;
    }

    private my_active_cell_move MyCheckCorrectCell(my_active_cell_move _source, int _direction)
    {
        //print("MyCheckCorrectCell");
        if (_source.myActiveCellMoveCollider[_direction] &&
            !_source.myActiveCellMoveCollider[_direction].MyIsTestBusy)
        {
            //print("end MyCheckCorrectCell");
            return _source.myActiveCellMoveCollider[_direction];
        }

        //print("end null MyCheckCorrectCell");
        return null;
    }

    public void MySetBusy(bool _isBusy)
    {
        MyIsBusyCell = _isBusy;
        MyIsTestBusy = _isBusy;

        if (MyIsBusyCell == false)
            myGem = null;
    }

    public bool MyIsHaveAMove()
    {
        if (myGem)
            return myGem.MyDoIHaveAMove(this);

        return false;
    }

    public void MyTryToMoveGemDownV2()
    {
        if (myGem)
            myGem.MyTryMoveDownV2(this);
    }

    public void MyAddedGem(my_gem _myGem)
    {
        myGem = _myGem;
        myGem.transform.SetParent(transform, false);

        MySetBusy(true);
    }

    public void MyOnGemDestroy()
    {
        if (myGem && MyIsHaveGem)
            Destroy(transform.GetChild(0).gameObject);
        else
            print("exeption destroy");

        MySetBusy(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        myPositionMouse = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (MyIsHaveGem && (myTimeCurrent > myTimeStop))
        {
            myTimeCurrent = 0;
            myOldPositionMouse = eventData.position;

            Vector2 mouseDir = (myPositionMouse - myOldPositionMouse).normalized;
            MyDirection dir = MyDirection.UNKNOWN;

            if (mouseDir.x > 0.9f)
                dir = MyDirection.LEFT;
            else if (mouseDir.x < -0.9f)
                dir = MyDirection.RIGHT;
            else if (mouseDir.y > 0.9f)
                dir = MyDirection.DOWN;
            else if (mouseDir.y < -0.9f)
                dir = MyDirection.UP;

            MyEventMouseClickOnCell?.Invoke(this, dir);
        }
    }

    private void FixedUpdate()
    {
        myTimeCurrent += Time.deltaTime;
    }
}
