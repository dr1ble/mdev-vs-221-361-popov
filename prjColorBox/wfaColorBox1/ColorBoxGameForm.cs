using prjColorBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wfaColorBox
{
    public partial class ColorBoxGameForm : Form
    {
        // Константы для настроек игры
        private const int GRID_ROWS = 7;
        private const int GRID_COLS = 12;
        private const int NUM_COLORS = 5;
        private int CELL_SIZE = 24; // Начальное значение, будет уточнено

        private ColorMapGenerator _mapGenerator;
        private MapData _currentMapData;
        private List<int> _expectedColorOrder;
        private PictureBox[,] _cellControls;

        // Определяем цвета UI для ваших числовых идентификаторов цветов
        // и их русские названия
        private Dictionary<int, (Color UiColor, string Name)> _uiColorNameMapping = new Dictionary<int, (Color, string)>
        {
            { 1, (Color.Red,    "Красный") },
            { 2, (Color.Green,  "Зеленый") },
            { 3, (Color.Blue,   "Синий") },
            { 4, (Color.Yellow, "Желтый") },
            { 5, (Color.Orange, "Оранжевый") },
            { 6, (Color.Purple, "Фиолетовый") }
        };
        private Color _defaultCellColor = Color.LightGray;

        public ColorBoxGameForm()
        {
            InitializeComponent(); // Вызов кода из ColorBoxGameForm.Designer.cs
            _mapGenerator = new ColorMapGenerator();

            ConfigureInitialLayout(); // Настройка макета один раз

            // События
            this.btnStartLevel.Click += new System.EventHandler(this.btnStartLevel_Click);
            this.Load += new System.EventHandler(this.ColorBoxGameForm_Load);
            this.Resize += new System.EventHandler(this.ColorBoxGameForm_Resize); 
        }

        private void ConfigureInitialLayout()
        {
            panelColorGrid.Anchor = AnchorStyles.Top; 

            lblStatus.AutoSize = false;
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;
            lblStatus.Height = 30;
            lblStatus.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            flowLayoutPanelButtons.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanelButtons.WrapContents = false;
            flowLayoutPanelButtons.AutoSize = true; 
            flowLayoutPanelButtons.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanelButtons.Anchor = AnchorStyles.Top; 

            btnStartLevel.Height = 40;
            btnStartLevel.Font = new Font(btnStartLevel.Font.FontFamily, 10, FontStyle.Bold);
            btnStartLevel.Dock = DockStyle.Bottom; 
        }

        private void AdjustControlsPositions()
        {
            if (panelColorGrid.Dock == DockStyle.None || panelColorGrid.Dock == DockStyle.Top) 
            {
                panelColorGrid.Left = (this.ClientSize.Width - panelColorGrid.Width) / 2;
            }
            
            lblStatus.Top = panelColorGrid.Bottom + 5;
            lblStatus.Left = 10; 
            lblStatus.Width = this.ClientSize.Width - 20; 

            flowLayoutPanelButtons.Top = lblStatus.Bottom + 5;
            if (flowLayoutPanelButtons.Visible) 
            {
                 flowLayoutPanelButtons.Left = (this.ClientSize.Width - flowLayoutPanelButtons.Width) / 2;
            }
        }

        private void InitializeGridControlsAndCellSize()
        {
            int availableHeight = panelColorGrid.Height; 
            int availableWidth = panelColorGrid.Width;   

            if (GRID_ROWS > 0 && availableHeight > 0)
            {
                CELL_SIZE = availableHeight / GRID_ROWS;
            }
            if (GRID_COLS > 0 && availableWidth > 0 && CELL_SIZE > 0) 
            {
                int cellSizeBasedOnWidth = availableWidth / GRID_COLS;
                CELL_SIZE = Math.Min(CELL_SIZE, cellSizeBasedOnWidth); 
            }
             if(CELL_SIZE < 10) CELL_SIZE = 10; 

            panelColorGrid.Width = GRID_COLS * CELL_SIZE;
            panelColorGrid.Height = GRID_ROWS * CELL_SIZE;
            
            panelColorGrid.Controls.Clear(); 
            _cellControls = new PictureBox[GRID_ROWS, GRID_COLS]; 

            for (int r = 0; r < GRID_ROWS; r++)
            {
                for (int c = 0; c < GRID_COLS; c++)
                {
                    PictureBox pb = new PictureBox
                    {
                        Width = CELL_SIZE,
                        Height = CELL_SIZE,
                        Location = new Point(c * CELL_SIZE, r * CELL_SIZE),
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = _defaultCellColor,
                        Visible = false 
                    };
                    _cellControls[r, c] = pb;
                    panelColorGrid.Controls.Add(pb);
                }
            }
        }

        private void btnStartLevel_Click(object sender, EventArgs e)
        {
            StartNewLevel();
        }

        private void StartNewLevel()
        {
            InitializeGridControlsAndCellSize(); 
            AdjustControlsPositions();           

            try
            {
                _currentMapData = _mapGenerator.GenerateMap(GRID_ROWS, GRID_COLS, NUM_COLORS);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Ошибка генерации карты: {ex.Message}", "Ошибка конфигурации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Ошибка! Проверьте настройки.";
                btnStartLevel.Text = "Начать новый уровень"; 
                btnStartLevel.Enabled = true;
                flowLayoutPanelButtons.Visible = false;
                return;
            }

            _expectedColorOrder = new List<int>(_currentMapData.SortedColorIDsByFrequency);

            DisplayMap(); 
            SetupColorButtons();
            lblStatus.Text = $"Уровень начат! Найдите самый частый цвет.";
            btnStartLevel.Text = "Перезапустить уровень";
            btnStartLevel.Enabled = true;
            flowLayoutPanelButtons.Visible = true;

            AdjustControlsPositions(); 
        }

        private void DisplayMap()
        {
            if (_currentMapData == null || _currentMapData.Grid == null || _cellControls == null) return;

            for (int r = 0; r < GRID_ROWS; r++)
            {
                for (int c = 0; c < GRID_COLS; c++)
                {
                    if (r < _cellControls.GetLength(0) && c < _cellControls.GetLength(1) && _cellControls[r,c] != null)
                    {
                        PictureBox pb = _cellControls[r, c];
                        if (r < _currentMapData.Grid.Length && c < _currentMapData.Grid[r].Length)
                        {
                            int colorId = _currentMapData.Grid[r][c];
                            if (colorId != 0 && _uiColorNameMapping.ContainsKey(colorId))
                            {
                                pb.BackColor = _uiColorNameMapping[colorId].UiColor;
                                pb.Tag = colorId;
                                pb.Visible = true;
                            }
                            else
                            {
                                pb.BackColor = _defaultCellColor; 
                                pb.Tag = 0;
                                pb.Visible = false; 
                            }
                        } else {
                             pb.Visible = false; 
                        }
                    }
                }
            }
        }

        private void SetupColorButtons()
        {
            flowLayoutPanelButtons.Controls.Clear();
            if (_currentMapData == null || _currentMapData.ColorCounts == null || !_currentMapData.ColorCounts.Any())
            {
                flowLayoutPanelButtons.Visible = false;
                return;
            }
            flowLayoutPanelButtons.Visible = true;

            var colorsInThisLevel = _currentMapData.ColorCounts.Keys.OrderBy(k => k);
            int buttonWidth = 90; 
            int buttonHeight = 35; 

            foreach (int colorId in colorsInThisLevel)
            {
                if (_uiColorNameMapping.ContainsKey(colorId))
                {
                    var colorInfo = _uiColorNameMapping[colorId];
                    Button btn = new Button
                    {
                        Text = colorInfo.Name,
                        Tag = colorId,
                        BackColor = colorInfo.UiColor,
                        ForeColor = IsDarkColor(colorInfo.UiColor) ? Color.White : Color.Black,
                        Size = new Size(buttonWidth, buttonHeight),
                        Margin = new Padding(3),
                        Font = new Font(this.Font.FontFamily, 9, FontStyle.Bold),
                        Enabled = true
                    };
                    btn.Click += ColorButton_Click;
                    flowLayoutPanelButtons.Controls.Add(btn);
                }
            }
        }

        private bool IsDarkColor(Color color)
        {
            return color.GetBrightness() < 0.5;
        }

        private void ColorButton_Click(object sender, EventArgs e)
        {
            if (_currentMapData == null || _expectedColorOrder == null || !_expectedColorOrder.Any()) return;

            Button clickedButton = sender as Button;
            if (clickedButton == null || clickedButton.Tag == null) return;

            int chosenColorId = (int)clickedButton.Tag;

            if (chosenColorId == _expectedColorOrder.First())
            {
                lblStatus.Text = $"Верно! Цвет '{_uiColorNameMapping[chosenColorId].Name}' удален.";
                _expectedColorOrder.RemoveAt(0);
                clickedButton.Enabled = false; 

                for (int r = 0; r < GRID_ROWS; r++)
                {
                    for (int c = 0; c < GRID_COLS; c++)
                    {
                        if (_cellControls[r, c] != null && _cellControls[r, c].Tag != null && (int)_cellControls[r, c].Tag == chosenColorId)
                        {
                            _cellControls[r, c].Visible = false;
                        }
                    }
                }

                if (!_expectedColorOrder.Any())
                {
                    lblStatus.Text = "Уровень пройден! Поздравляем!";
                    flowLayoutPanelButtons.Visible = false; 
                    Application.DoEvents(); 

                    MessageBox.Show("Уровень пройден! Следующий уровень...", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    StartNewLevel(); 
                }
                else
                {
                    lblStatus.Text += $" Теперь найдите следующий самый частый цвет.";
                }
            }
            else
            {
                lblStatus.Text = $"Неверно. Попробуйте еще раз. Ищем самый частый из оставшихся.";
            }
        }

        private void ColorBoxGameForm_Load(object sender, EventArgs e)
        {
            StartNewLevel(); 
        }

        private void ColorBoxGameForm_Resize(object sender, EventArgs e)
        {
            AdjustControlsPositions();
        }
    }
}