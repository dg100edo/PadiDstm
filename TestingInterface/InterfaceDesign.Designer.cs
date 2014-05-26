namespace TestingInterface
{
    partial class InterfaceDesign
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.ObjectIdBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.initButton = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.rectangleShape4 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.rectangleShape3 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.rectangleShape2 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.rectangleShape1 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.statusButton = new System.Windows.Forms.Button();
            this.failButton = new System.Windows.Forms.Button();
            this.freezeButton = new System.Windows.Forms.Button();
            this.recoverButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.urlBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ValueBox = new System.Windows.Forms.TextBox();
            this.button9 = new System.Windows.Forms.Button();
            this.AcessButton = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(400, 241);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // ObjectIdBox
            // 
            this.ObjectIdBox.Location = new System.Drawing.Point(715, 154);
            this.ObjectIdBox.Name = "ObjectIdBox";
            this.ObjectIdBox.Size = new System.Drawing.Size(75, 20);
            this.ObjectIdBox.TabIndex = 1;
            this.ObjectIdBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(642, 157);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Object ID :";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // initButton
            // 
            this.initButton.Location = new System.Drawing.Point(521, 33);
            this.initButton.Name = "initButton";
            this.initButton.Size = new System.Drawing.Size(75, 23);
            this.initButton.TabIndex = 3;
            this.initButton.Text = "Init";
            this.initButton.UseVisualStyleBackColor = true;
            this.initButton.Click += new System.EventHandler(this.init_button_click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(440, 147);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "TxBegin";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.tx_init_button_click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(521, 147);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "TxCommit";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.tx_commit_button_click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(521, 188);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "TxAbort";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.tx_abort_button_click);
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.rectangleShape4,
            this.rectangleShape3,
            this.rectangleShape2,
            this.rectangleShape1});
            this.shapeContainer1.Size = new System.Drawing.Size(985, 265);
            this.shapeContainer1.TabIndex = 7;
            this.shapeContainer1.TabStop = false;
            // 
            // rectangleShape4
            // 
            this.rectangleShape4.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.rectangleShape4.Location = new System.Drawing.Point(628, 127);
            this.rectangleShape4.Name = "rectangleShape4";
            this.rectangleShape4.Size = new System.Drawing.Size(343, 109);
            this.rectangleShape4.Click += new System.EventHandler(this.rectangleShape4_Click);
            // 
            // rectangleShape3
            // 
            this.rectangleShape3.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.rectangleShape3.Location = new System.Drawing.Point(508, 18);
            this.rectangleShape3.Name = "rectangleShape3";
            this.rectangleShape3.Size = new System.Drawing.Size(99, 93);
            this.rectangleShape3.Click += new System.EventHandler(this.rectangleShape3_Click);
            // 
            // rectangleShape2
            // 
            this.rectangleShape2.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.rectangleShape2.Location = new System.Drawing.Point(628, 16);
            this.rectangleShape2.Name = "rectangleShape2";
            this.rectangleShape2.Size = new System.Drawing.Size(342, 95);
            this.rectangleShape2.Click += new System.EventHandler(this.rectangleShape2_Click);
            // 
            // rectangleShape1
            // 
            this.rectangleShape1.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.rectangleShape1.Location = new System.Drawing.Point(432, 129);
            this.rectangleShape1.Name = "rectangleShape1";
            this.rectangleShape1.Size = new System.Drawing.Size(175, 105);
            this.rectangleShape1.Click += new System.EventHandler(this.rectangleShape1_Click);
            // 
            // statusButton
            // 
            this.statusButton.Location = new System.Drawing.Point(521, 77);
            this.statusButton.Name = "statusButton";
            this.statusButton.Size = new System.Drawing.Size(75, 23);
            this.statusButton.TabIndex = 8;
            this.statusButton.Text = "Status";
            this.statusButton.UseVisualStyleBackColor = true;
            this.statusButton.Click += new System.EventHandler(this.status_button_click);
            // 
            // failButton
            // 
            this.failButton.Location = new System.Drawing.Point(715, 77);
            this.failButton.Name = "failButton";
            this.failButton.Size = new System.Drawing.Size(75, 23);
            this.failButton.TabIndex = 9;
            this.failButton.Text = "Fail";
            this.failButton.UseVisualStyleBackColor = true;
            this.failButton.Click += new System.EventHandler(this.fail_button_click);
            // 
            // freezeButton
            // 
            this.freezeButton.Location = new System.Drawing.Point(802, 77);
            this.freezeButton.Name = "freezeButton";
            this.freezeButton.Size = new System.Drawing.Size(75, 23);
            this.freezeButton.TabIndex = 10;
            this.freezeButton.Text = "Freeze";
            this.freezeButton.UseVisualStyleBackColor = true;
            this.freezeButton.Click += new System.EventHandler(this.freeze_button_click);
            // 
            // recoverButton
            // 
            this.recoverButton.Location = new System.Drawing.Point(883, 77);
            this.recoverButton.Name = "recoverButton";
            this.recoverButton.Size = new System.Drawing.Size(75, 23);
            this.recoverButton.TabIndex = 11;
            this.recoverButton.Text = "Recover";
            this.recoverButton.UseVisualStyleBackColor = true;
            this.recoverButton.Click += new System.EventHandler(this.recover_button_click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(642, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "URL  :";
            this.label2.Click += new System.EventHandler(this.url_label_click);
            // 
            // urlBox
            // 
            this.urlBox.Location = new System.Drawing.Point(715, 30);
            this.urlBox.Name = "urlBox";
            this.urlBox.Size = new System.Drawing.Size(243, 20);
            this.urlBox.TabIndex = 13;
            this.urlBox.Text = "tcp://localhost:XX/Server";
            this.urlBox.TextChanged += new System.EventHandler(this.url_textbox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(642, 193);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Value :";
            // 
            // ValueBox
            // 
            this.ValueBox.Location = new System.Drawing.Point(715, 190);
            this.ValueBox.Name = "ValueBox";
            this.ValueBox.Size = new System.Drawing.Size(75, 20);
            this.ValueBox.TabIndex = 15;
            this.ValueBox.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(883, 152);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(75, 23);
            this.button9.TabIndex = 16;
            this.button9.Text = "Create";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.create_button_click);
            // 
            // AcessButton
            // 
            this.AcessButton.Location = new System.Drawing.Point(802, 152);
            this.AcessButton.Name = "AcessButton";
            this.AcessButton.Size = new System.Drawing.Size(75, 23);
            this.AcessButton.TabIndex = 17;
            this.AcessButton.Text = "Acess";
            this.AcessButton.UseVisualStyleBackColor = true;
            this.AcessButton.Click += new System.EventHandler(this.acess_button_click);
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(802, 190);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(75, 23);
            this.button11.TabIndex = 18;
            this.button11.Text = "Read";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.read_button_click);
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(883, 190);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(75, 23);
            this.button12.TabIndex = 19;
            this.button12.Text = "Write";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.write_button_click);
            // 
            // InterfaceDesign
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(985, 265);
            this.Controls.Add(this.button12);
            this.Controls.Add(this.button11);
            this.Controls.Add(this.AcessButton);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.ValueBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.urlBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.recoverButton);
            this.Controls.Add(this.freezeButton);
            this.Controls.Add(this.failButton);
            this.Controls.Add(this.statusButton);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.initButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ObjectIdBox);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.shapeContainer1);
            this.Name = "InterfaceDesign";
            this.Text = "Padi DSTM Client Interface";
            this.Load += new System.EventHandler(this.InterfaceDesign_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TextBox ObjectIdBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button initButton;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape2;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape1;
        private System.Windows.Forms.Button statusButton;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape3;
        private System.Windows.Forms.Button failButton;
        private System.Windows.Forms.Button freezeButton;
        private System.Windows.Forms.Button recoverButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox urlBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ValueBox;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button AcessButton;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button12;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape4;
    }
}

