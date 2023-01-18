using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace posk6
{
    public partial class Form1 : Form
    {
        private Board gameBoard;
        private Choice choiceBoard;

        public Form1()
        {
            InitializeComponent();
            gameBoard = new Board(board);
            choiceBoard = new Choice(choice, new EventHandler(choice_Click));
        }

        private void choice_Click(object sender, EventArgs e)
        {
            gameBoard.InsertToken(Convert.ToInt32(((Button)sender).Name));
        }
    }
    public class Board
    {
        private List<List<Label>> columns;

        public Board(TableLayoutPanel board)
        {
            columns = new List<List<Label>>();
            for(int i = 0; i < 7; i++)
            {
                columns.Add(new List<Label>());
                for (int j = 0; j < 6; j++)
                {
                    columns[i].Add(new Label());
                    columns[i][j].BackColor = Color.Gray;
                    columns[i][j].Dock = DockStyle.Fill;
                    board.Controls.Add(columns[i][j], i, j);
                }
            }
        }
        public void InsertToken(int column)
        {
            int row = columns[column].Count - (1 + GetFilledRows(column));
            columns[column][row].BackColor = Color.Blue;
        }
        public int GetFilledRows(int column)
        {
            int count = columns[column].Count;
            foreach(var row in columns[column])
            {
                if (row.BackColor == Color.Gray) { count--; }
            }
            return count;
        }
    }
    public class Choice
    {
        private List<Button> buttons;

        public Choice(TableLayoutPanel choice, EventHandler choice_Handler)
        {
            buttons = new List<Button>();
            for (int i = 0; i < choice.ColumnCount; i++)
            {
                buttons.Add(new Button());
                choice.Controls.Add(buttons[i]);
                buttons[i].Click += choice_Handler;
                buttons[i].Dock = DockStyle.Fill;
                buttons[i].Text = "▼";
                buttons[i].Margin = Padding.Empty;
                buttons[i].Name = i.ToString();
            }
        }
    }
}
