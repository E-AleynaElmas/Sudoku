using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuGrid : MonoBehaviour
{
    public int columns = 0;
    public int rows = 0;
    public float sqaure_offset = 0.0f;
    public GameObject grid_sqaure;
    public Vector2 start_position = new Vector2(0.0f, 0.0f);
    public float sqaure_scale = 1.0f;
    public float square_gap = 0.1f;
    public Color line_highlight_color = Color.red;

    public List<GameObject> grid_sqaures_ = new List<GameObject>();
    private int selected_grid_data = - 1;

    void Start()
    {
        if (grid_sqaure.GetComponent<GridSqaure>() == null)
        {
            Debug.LogError("This Game Object need to have GridSqaure script attached !");
        }

        //CreateGrid();
        //SetPositionNumber();

        if (GameSettings.Instance.GetContinuePreviousGame())
            SetGridFormFile();

        else
            SetGridNumber(GameSettings.Instance.GetGameMode());

        //SetGridNumber(GameSettings.Instance.GetGameMode());//her grid square için numaralandırma
    }

    void SetGridFormFile()
    {
        string level = GameSettings.Instance.GetGameMode();
        selected_grid_data = Config.ReadGameBoardLevel();
        var data = Config.ReadGridData();

        setGridSqaure(data);
        SetGridNotes(Config.GetGridNotes());

;    }

    private void SetGridNotes(Dictionary<int,List<int>> notes)
    {
        foreach(var note in notes)
        {
            grid_sqaures_[note.Key].GetComponent<GridSqaure>().SetGridNotes(note.Value);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    //private void CreateGrid()
    //{
    //    SpawnGridSqaures();
    //    SetSqauresPosition();
    //}

    //private void SpawnGridSqaures()
    //{
    //    //0, 1, 2, 3, 4, 5, 6,
    //    //7,8, ....
    //    int square_index = 0;
    //    for (int row = 0; row < rows; row++)
    //    {
    //        for (int column = 0; column < columns; column++)
    //        {
    //            grid_sqaures_.Add(Instantiate(grid_sqaure) as GameObject);
    //            grid_sqaures_[grid_sqaures_.Count - 1].GetComponent<GridSqaure>().SetSquareIndex(square_index);
    //            grid_sqaures_[grid_sqaures_.Count - 1].transform.parent = this.transform;
    //            grid_sqaures_[grid_sqaures_.Count - 1].transform.localScale = new Vector3(sqaure_scale, sqaure_scale, sqaure_scale);

    //            square_index++;
    //        }
    //    }
    //}
    //private void SetPositionNumber()
    //{
    //    int square_index = 0;
    //    for (int row = 0; row < rows; row++)
    //    {
    //        for (int column = 0; column < columns; column++)
    //        {
    //            grid_sqaures_[grid_sqaures_.Count - 1].GetComponent<GridSqaure>().SetSquareIndex(square_index);
    //            square_index++;
    //        }
    //    }
    //}
    //private void SetSqauresPosition()
    //{
    //    var sqaure_rect = grid_sqaures_[0].GetComponent<RectTransform>();
    //    Vector2 offset = new Vector2();
    //    Vector2 square_gap_number = new Vector2(0.0f, 0.0f);
    //    bool row_moved = false;

    //    offset.x = sqaure_rect.rect.width * sqaure_rect.transform.localScale.x + sqaure_offset;
    //    offset.y = sqaure_rect.rect.height * sqaure_rect.transform.localScale.y + sqaure_offset;

    //    int column_number = 0;
    //    int row_number = 0;

    //    foreach (GameObject sqaure in grid_sqaures_)
    //    {
    //        if (column_number + 1 > columns)
    //        {
    //            row_number++;
    //            column_number = 0;
    //            square_gap_number.x = 0;
    //            row_moved = false;
    //        }

    //        var pos_x_offset = offset.x * column_number + (square_gap_number.x * square_gap);
    //        var pos_y_offset = offset.y * row_number + (square_gap_number.y * square_gap);

    //        if (column_number > 0 && column_number % 3 == 0)
    //        {
    //            square_gap_number.x++;
    //            pos_x_offset += square_gap;
    //        }

    //        if (row_number > 0 && row_number % 3 == 0 && row_moved == false)
    //        {
    //            row_moved = true;
    //            square_gap_number.y++;
    //            pos_y_offset += square_gap;
    //        }

    //        sqaure.GetComponent<RectTransform>().anchoredPosition = new Vector2(start_position.x + pos_x_offset, start_position.y - pos_y_offset);
    //        column_number++;
    //    }
    //}

    private void SetGridNumber(string level)
    {

        selected_grid_data = Random.Range(0, SudokuData.Instance.sudoku_game[level].Count);
        var data = SudokuData.Instance.sudoku_game[level][selected_grid_data];

        setGridSqaure(data);
        //foreach (var square in grid_sqaures_)
        //{
        //    square.GetComponent<GridSqaure>().SetNumber(Random.Range(0, 10));
        //}
    }

    private void setGridSqaure(SudokuData.SudokuBoardData data)
    {
        for(int index = 0; index < grid_sqaures_.Count; index++)
        {
            grid_sqaures_[index].GetComponent<GridSqaure>().SetNumber(data.unsolved_data[index]);
            grid_sqaures_[index].GetComponent<GridSqaure>().SetCorrectNumber(data.solved_data[index]);
            grid_sqaures_[index].GetComponent<GridSqaure>().SetHasDefaultValue(data.unsolved_data[index] != 0 && data.unsolved_data[index] == data.solved_data[index]);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnSquareSelected += OnSquareSelected;
        GameEvents.OnUpdateSquareNumber += CheckBoardCompleted;
    }

    private void OnDisable()
    {
        GameEvents.OnSquareSelected -= OnSquareSelected;
        GameEvents.OnUpdateSquareNumber -= CheckBoardCompleted;

        //************************************
        var solved_data = SudokuData.Instance.sudoku_game[GameSettings.Instance.GetGameMode()][selected_grid_data].solved_data;
        int[] unsolved_data = new int[81];
        Dictionary<string, List<string>> grid_notes = new Dictionary<string, List<string>>();
        for(int i = 0; i < grid_sqaures_.Count; i++)
        {
            var comp = grid_sqaures_[i].GetComponent<GridSqaure>();
            unsolved_data[i] = comp.GetSquareNumber();
            string key = "square_note:" + i.ToString();
            grid_notes.Add(key, comp.GetSquareNotes());
        }

        SudokuData.SudokuBoardData current_game_data = new SudokuData.SudokuBoardData(unsolved_data,solved_data);
        if (GameSettings.Instance.GetExitAfterWon() == false)//dont save data when exit after completed board
        {
            Config.SaveBoardData(current_game_data,
                GameSettings.Instance.GetGameMode(),
                selected_grid_data,
                Lives.instance.GetErrorNumbers(),
                grid_notes);
        }

        else
            Config.DeleteDataFile();

        GameSettings.Instance.SetExitAfterWon(false);
    }

    private void SetSquaresColor(int[] data, Color col)
    {
        foreach(var index in data)
        {
            var comp = grid_sqaures_[index].GetComponent<GridSqaure>();
            if (comp.HasWrongValue()==false && comp.IsSelected() == false)
            {
                comp.SetSquareColor(col);
            }
        }
    }

    public void OnSquareSelected(int square_index)
    {
        var horizontal_line = LineIndicator.instance.GetHorizantalLine(square_index);
        var vertical_line = LineIndicator.instance.GetVerticalLine(square_index);
        var square = LineIndicator.instance.GetSquare(square_index);

        SetSquaresColor(LineIndicator.instance.GetAllSquaresIndexes(), Color.white);

        SetSquaresColor(horizontal_line, line_highlight_color);
        SetSquaresColor(vertical_line, line_highlight_color);
        SetSquaresColor(square, line_highlight_color);


    }

    private void CheckBoardCompleted(int number)
    {
        foreach(var square in grid_sqaures_)
        {
            var comp = square.GetComponent<GridSqaure>();
            if (comp.IsCorrectNumberSet() == false)
                return;
        }

        GameEvents.OnBoardCompletedMethod();
    }

    public void SolveSudoku()
    {
        foreach(var square in grid_sqaures_)
        {
            var comp = square.GetComponent<GridSqaure>();
            comp.SetCorrectNumber();
        }

        CheckBoardCompleted(0);
    }
}
