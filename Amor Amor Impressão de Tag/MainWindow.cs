using System;
using System.Windows.Forms;

public partial class MainWindow : Form
{
    private TextBox productSearchTextBox;
    private Button searchButton;
    private Button printButton;
    private Button optionsButton;
    private Label productInfoLabel;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        productSearchTextBox = new TextBox();
        searchButton = new Button();
        printButton = new Button();
        optionsButton = new Button();
        productInfoLabel = new Label();

        // Product Search TextBox
        productSearchTextBox.Location = new System.Drawing.Point(13, 13);
        productSearchTextBox.Name = "productSearchTextBox";
        productSearchTextBox.Size = new System.Drawing.Size(300, 20);
        productSearchTextBox.TabIndex = 0;

        // Search Button
        searchButton.Location = new System.Drawing.Point(320, 13);
        searchButton.Name = "searchButton";
        searchButton.Size = new System.Drawing.Size(75, 23);
        searchButton.TabIndex = 1;
        searchButton.Text = "Search";
        searchButton.UseVisualStyleBackColor = true;
        searchButton.Click += new EventHandler(SearchButton_Click);

        // Print Button
        printButton.Location = new System.Drawing.Point(320, 43);
        printButton.Name = "printButton";
        printButton.Size = new System.Drawing.Size(75, 23);
        printButton.TabIndex = 2;
        printButton.Text = "Print";
        printButton.UseVisualStyleBackColor = true;
        printButton.Click += new EventHandler(PrintButton_Click);

        // Options Button
        optionsButton.Location = new System.Drawing.Point(320, 73);
        optionsButton.Name = "optionsButton";
        optionsButton.Size = new System.Drawing.Size(75, 23);
        optionsButton.TabIndex = 3;
        optionsButton.Text = "Options";
        optionsButton.UseVisualStyleBackColor = true;
        optionsButton.Click += new EventHandler(OptionsButton_Click);

        // Product Info Label
        productInfoLabel.Location = new System.Drawing.Point(13, 43);
        productInfoLabel.Name = "productInfoLabel";
        productInfoLabel.Size = new System.Drawing.Size(300, 200);
        productInfoLabel.TabIndex = 4;

        // MainWindow
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(408, 261);
        this.Controls.Add(productInfoLabel);
        this.Controls.Add(optionsButton);
        this.Controls.Add(printButton);
        this.Controls.Add(searchButton);
        this.Controls.Add(productSearchTextBox);
        this.Name = "MainWindow";
        this.Text = "Product Search";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private void SearchButton_Click(object sender, EventArgs e)
    {
        // Handle search button click.
        // Here is where you would typically initiate the API call.
    }

    private void PrintButton_Click(object sender, EventArgs e)
    {
        // Handle print button click.
        // Here is where you would typically send the ZPL to the printer.
    }

    private void OptionsButton_Click(object sender, EventArgs e)
    {
        // Handle options button click.
        // Here is where you would typically open the options dialog.
    }
}
