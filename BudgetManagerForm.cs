using System.Data;
using System.Windows.Forms;
using FamilyBudgetManager.TransactionsRepository;

namespace FamilyBudgetManager
{
    public partial class BudgetManagerForm : Form
    {
        private readonly ITransactionRepository _repository;
        public BudgetManagerForm(ITransactionRepository repository)
        {
            _repository = repository;
            InitializeComponent();
            CreateDataBaseIfNotExists();
            LoadTransactions();
            dataGridView.SelectionChanged += DataGridView_SelectionChanged;
            AmountTextBox.KeyPress += AmountTextBox_OnlyPositiveInteger_KeyPress;
        }

        private void AmountTextBox_OnlyPositiveInteger_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void DataGridView_SelectionChanged(object? sender, EventArgs e)
        {
            if(!IsRowSelected(dataGridView)) return;

            var row = dataGridView.SelectedRows[0];

            CategoryComboBox.SelectedItem = row.Cells["category"].Value.ToString();
            DescriptionTextBox.Text = row.Cells["description"].Value.ToString();
            AmountTextBox.Text = row.Cells["amount"].Value.ToString();
            DatePicker.Value = DateTime.TryParse(row.Cells["date"].Value.ToString(), out DateTime date) 
                ? date 
                : DateTime.Today;
        }

        private void CreateDataBaseIfNotExists()
        {
            _repository.CreateNewIfNotExists();
        }

        private void InitializeComponent()
        {
            CategoryLabel = new Label();
            CategoryComboBox = new ComboBox();
            DescriptionLabel = new Label();
            DescriptionTextBox = new TextBox();
            AmountLabel = new Label();
            AmountTextBox = new TextBox();
            DateLabel = new Label();
            DatePicker = new DateTimePicker();
            dataGridView = new DataGridView();
            AddTransactionButton = new Button();
            RemoveTransactionButton = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
            SuspendLayout();
            // 
            // CategoryLabel
            // 
            CategoryLabel.AutoSize = true;
            CategoryLabel.Location = new Point(25, 380);
            CategoryLabel.Name = "CategoryLabel";
            CategoryLabel.Size = new Size(55, 15);
            CategoryLabel.TabIndex = 0;
            CategoryLabel.Text = "Category";
            // 
            // CategoryComboBox
            // 
            CategoryComboBox.FormattingEnabled = true;
            CategoryComboBox.Items.AddRange(new object[] { "", "Income", "Expense" });
            CategoryComboBox.Location = new Point(97, 377);
            CategoryComboBox.Name = "CategoryComboBox";
            CategoryComboBox.Size = new Size(151, 23);
            CategoryComboBox.TabIndex = 1;
            CategoryComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            // 
            // DescriptionLabel
            // 
            DescriptionLabel.AutoSize = true;
            DescriptionLabel.Location = new Point(25, 412);
            DescriptionLabel.Name = "DescriptionLabel";
            DescriptionLabel.Size = new Size(67, 15);
            DescriptionLabel.TabIndex = 2;
            DescriptionLabel.Text = "Description";
            // 
            // DescriptionTextBox
            // 
            DescriptionTextBox.Location = new Point(97, 409);
            DescriptionTextBox.Name = "DescriptionTextBox";
            DescriptionTextBox.Size = new Size(567, 23);
            DescriptionTextBox.TabIndex = 3;
            // 
            // AmountLabel
            // 
            AmountLabel.AutoSize = true;
            AmountLabel.Location = new Point(25, 441);
            AmountLabel.Name = "AmountLabel";
            AmountLabel.Size = new Size(51, 15);
            AmountLabel.TabIndex = 4;
            AmountLabel.Text = "Amount";
            // 
            // AmountTextBox
            // 
            AmountTextBox.Location = new Point(97, 438);
            AmountTextBox.Name = "AmountTextBox";
            AmountTextBox.Size = new Size(567, 23);
            AmountTextBox.TabIndex = 5;
            // 
            // DateLabel
            // 
            DateLabel.AutoSize = true;
            DateLabel.Location = new Point(25, 473);
            DateLabel.Name = "DateLabel";
            DateLabel.Size = new Size(31, 15);
            DateLabel.TabIndex = 6;
            DateLabel.Text = "Date";
            // 
            // DatePicker
            // 
            DatePicker.Location = new Point(97, 467);
            DatePicker.Name = "DatePicker";
            DatePicker.Size = new Size(200, 23);
            DatePicker.TabIndex = 7;
            // 
            // dataGridView
            // 
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Location = new Point(25, 12);
            dataGridView.Name = "dataGridView";
            dataGridView.RowTemplate.Height = 25;
            dataGridView.Size = new Size(639, 320);
            dataGridView.TabIndex = 8;
            // 
            // AddTransactionButton
            // 
            AddTransactionButton.Location = new Point(27, 502);
            AddTransactionButton.Name = "AddTransactionButton";
            AddTransactionButton.Size = new Size(113, 29);
            AddTransactionButton.TabIndex = 9;
            AddTransactionButton.Text = "Add Transaction";
            AddTransactionButton.UseVisualStyleBackColor = true;
            AddTransactionButton.Click += AddTransactionButton_Click;
            // 
            // RemoveTransactionButton
            // 
            RemoveTransactionButton.Location = new Point(146, 502);
            RemoveTransactionButton.Name = "RemoveTransactionButton";
            RemoveTransactionButton.Size = new Size(126, 29);
            RemoveTransactionButton.TabIndex = 10;
            RemoveTransactionButton.Text = "Remove Transaction";
            RemoveTransactionButton.UseVisualStyleBackColor = true;
            RemoveTransactionButton.Click += RemoveTransactionButton_Click;
            // 
            // BudgetManagerForm
            // 
            ClientSize = new Size(688, 543);
            Controls.Add(RemoveTransactionButton);
            Controls.Add(AddTransactionButton);
            Controls.Add(dataGridView);
            Controls.Add(DatePicker);
            Controls.Add(DateLabel);
            Controls.Add(AmountTextBox);
            Controls.Add(AmountLabel);
            Controls.Add(DescriptionTextBox);
            Controls.Add(DescriptionLabel);
            Controls.Add(CategoryComboBox);
            Controls.Add(CategoryLabel);
            Name = "BudgetManagerForm";
            Text = "Family Budget Manager";
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private void AddTransactionButton_Click(object sender, EventArgs e)
        {
            AddTransaction();
        }

        private void RemoveTransactionButton_Click(object sender, EventArgs e)
        {
            RemoveTransaction();
        }

        private void AddTransaction()
        {
            string category = CategoryComboBox.SelectedItem?.ToString();
            string description = DescriptionTextBox.Text.Trim();
            string amount = AmountTextBox.Text.Trim();
            DateTime date = DatePicker.Value;

            if (IsInputInvalid(category, description, amount))
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            _repository.Write(category, description, amount, date);
            LoadTransactions();
        }

        private static bool IsInputInvalid(string category, string description, string amount)
        {
            if(string.IsNullOrEmpty(category) || 
               string.IsNullOrEmpty(description) || 
               string.IsNullOrEmpty(amount))
            {
                return true;
            }
            return false;
        }

        private void LoadTransactions()
        {
            DataTable table = _repository.ReadAllTransactions();
            BindDataTable(table);
            SetupDataGridViewAppearance(dataGridView);
        }

        private void SetupDataGridViewAppearance(DataGridView dataGridView)
        {
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.RowHeadersVisible = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.MultiSelect = false;
            dataGridView.ReadOnly = true;
        }

        private void BindDataTable(DataTable table)
        {
            dataGridView.DataSource = table;
        }

        private void RemoveTransaction()
        {
            if (!GetSelectedRowId(dataGridView, out int id))
                return;

            var confirm = MessageBox.Show(
                "Are you sure you want to remove this transaction?",
                "Remove Transaction",
                MessageBoxButtons.YesNo);
            if (confirm != DialogResult.Yes) return;

            _repository.Delete(id);
            LoadTransactions();
        }

        private static bool GetSelectedRowId(DataGridView dataGridView, out int id)
        {
            id = -1;

            if (!IsRowSelected(dataGridView))
            {
                MessageBox.Show("Please select a row to remove.");
                return false;
            }

            DataGridViewRow selectedRow = dataGridView.SelectedRows[0];

            if (!dataGridView.Columns.Contains("id"))
            {
                MessageBox.Show("The table doesn't contain an 'id' column.");
                return false;
            }

            object? idValue = selectedRow.Cells["id"].Value;
            if (idValue == null || !int.TryParse(idValue.ToString(), out id))
            {
                MessageBox.Show("Invalid row selected.");
                return false;
            }

            return true;
        }

        private static bool IsRowSelected(DataGridView dataGridView)
        {
            return dataGridView.SelectedRows.Count != 0;
        }
    }
}
