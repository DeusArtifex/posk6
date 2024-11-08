﻿using System;
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
        private Color playerColor;
        private Color playerOne = Color.CornflowerBlue;
        private Color playerTwo = Color.Coral;

        public Form1()
        {
            InitializeComponent();
            gameBoard = new Board(board);                                       //Utworzenie planszy w podanej tabeli
            choiceBoard = new Choice(choice, new EventHandler(choice_Click));   //Utworzenie przycisków wyboru w podanej tabeli
            playerColor = playerOne;
            panel1.BackColor = playerColor;
        }

        private void choice_Click(object sender, EventArgs e)
        {
            int filledCol = Convert.ToInt32(((Button)sender).Name);             //Kliknięcie zmienia kolor kolejnego wolnego pola w kolumnie -
            int filledRow = gameBoard.InsertToken(filledCol, playerColor);      // - potem sprawdza, czy ruch nie spowodował wygtanej, a na -
            bool colFilled = gameBoard.GetFilledRows(filledCol) == 6;           // - końcu zmienia kolor aktualnego gracza.
            if (colFilled) { ((Button)sender).Enabled = false; }
            if (gameBoard.WinConditions(filledCol, filledRow, playerColor))
            {
                choice.Enabled = false;
                label1.Text = "Wygrywa:";       //W przypadku wygranej, przyciski się blokują, a tekst zmienia się by ukazywać zwycięzce
                return;
            }
            if(playerColor == playerOne)
            {
                playerColor = playerTwo;
            }
            else
            {
                playerColor = playerOne;
            }
            panel1.BackColor = playerColor;
        }
    }
    public class Board          //Obiekt planszy
    {
        private List<List<Label>> columns;

        public Board(TableLayoutPanel board)
        {
            columns = new List<List<Label>>();
            for(int i = 0; i < 7; i++)
            {
                columns.Add(new List<Label>());         //Plansza gry jest listą list, w konstruktorze listy odpowiedzialne za kolumny, wypełniane -
                for (int j = 0; j < 6; j++)             // - są pustymi polami tekstowymi, których kolory będą zmieniane podczas gry
                {
                    columns[i].Add(new Label());
                    columns[i][j].BackColor = Color.Gray;
                    columns[i][j].Dock = DockStyle.Fill;
                    board.Controls.Add(columns[i][j], i, j);    //Utworzone pole tekstowe przypisane jest do odpowiedniej kolumny i szeregu tabeli
                }
            }
        }
        public int InsertToken(int column, Color color)         //"Token" jest wpisywany do pierwszego wolnego miejsca w kolumnie. W tym celu od jej długości -
        {                                                       // - odejmowana jest ilość wypełnionych pól, zyskana z metody
            int row = columns[column].Count - (1 + GetFilledRows(column));
            columns[column][row].BackColor = color;
            return row;
        }
        public int GetFilledRows(int column)                    //Zlicza pola których kolor tła nie jest taki jak poczatkowy (szary)
        {
            int count = columns[column].Count;
            foreach(var row in columns[column])
            {
                if (row.BackColor == Color.Gray) { count--; }
            }
            return count;
        }
        public bool WinConditions(int col, int row, Color color)    //Funkcja sprawdza warunki dla kolumny, szeregu i skosów, zwraca prawdę -
        {                                                           // - jeżeli którykolwiek warunek został spełniony
            bool rows = CheckRow(row, color);
            bool columns = CheckCol(col, color);
            bool diags = CheckDiags(col, row, color);
            return rows || columns || diags;
        }
        private bool CheckRow(int row, Color color)
        {
            int count = 0;
            foreach(var column in columns)                      //szeregi sprawdzają zadany szereg w każdej kolumnie, 
            {
                if (column[row].BackColor == color){ count++; }
                else { count = 0; }
                if (count == 4) { return true; }
            }
            return false;
        }
        private bool CheckCol(int col, Color color)
        {
            int count = 0;
            foreach (var row in columns[col])
            {
                if (row.BackColor == color) { count++; }
                else { count = 0; }
                if (count == 4) { return true; }
            }
            return false;
        }
        private bool CheckDiags(int col, int row, Color color)
        {
            List<Label> diag1 = GetDiag(col, row, 1);
            List<Label> diag2 = GetDiag(col, row, -1);
            if(CheckDiag(diag1, color) || CheckDiag(diag2, color)) { return true; }
            return false;
        }
        private bool CheckDiag(List<Label> diag, Color color)
        {
            int count = 0;
            if(diag == null) { return false; }
            foreach (var field in diag)
            {
                if(field.BackColor == color) { count++; }
                else { count = 0; }
                if (count == 4) { return true; }
            }
            return false;
        }
        private List<Label> GetDiag(int col, int row, int dirLR)
        {
            int currRow = row;
            int currCol = col;
            int dirUD = 1;
            bool bounced = false;
            List<Label> diag = new List<Label>();
            for(int i = 0; i < 6; i++)
            {
                if (bounced)
                {
                    try
                    {
                        diag.Add(columns[currCol][currRow]);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        break;
                    }
                }
                if(i == 5) { break; }
                int newRow = currRow + dirUD;
                int newCol = currCol + dirLR;
                bool maxRow = newRow == 6;
                bool maxCol = newCol == 7;
                bool minRow = currRow == 0 && newRow < 0;
                bool minCol = currCol == 0 && newCol < 0;
                if (minRow || minCol || maxCol || maxRow)
                {
                    if (bounced)
                    {
                        break;
                    }
                    i = 0;
                    dirUD *= -1;
                    dirLR *= -1;
                    bounced = true;
                    continue;
                }
                currCol = newCol;
                currRow = newRow;
            }
            if(diag.Count < 4) { return null; }
            else { return diag; }

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
