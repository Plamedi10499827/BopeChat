namespace BopeChat
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            button2 = new Button();
            pictureBox1 = new PictureBox();
            ChatInput = new TextBox();
            ChatInput2 = new TextBox();
            button3 = new Button();
            button4 = new Button();
            radioButton1 = new RadioButton();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(1, 1);
            button1.Name = "button1";
            button1.Size = new Size(112, 34);
            button1.TabIndex = 0;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button2.Location = new Point(390, 404);
            button2.Name = "button2";
            button2.Size = new Size(112, 34);
            button2.TabIndex = 1;
            button2.Text = "Start!";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(1, 1);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(184, 151);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // ChatInput
            // 
            ChatInput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ChatInput.BackColor = Color.MidnightBlue;
            ChatInput.Location = new Point(191, 69);
            ChatInput.Multiline = true;
            ChatInput.Name = "ChatInput";
            ChatInput.ReadOnly = true;
            ChatInput.ScrollBars = ScrollBars.Vertical;
            ChatInput.Size = new Size(571, 280);
            ChatInput.TabIndex = 3;
            ChatInput.TextChanged += ChatInput_TextChanged;
            ChatInput.KeyDown += ChatInput_KeyDown;
            // 
            // ChatInput2
            // 
            ChatInput2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ChatInput2.BackColor = SystemColors.ActiveBorder;
            ChatInput2.Location = new Point(191, 355);
            ChatInput2.Name = "ChatInput2";
            ChatInput2.Size = new Size(486, 31);
            ChatInput2.TabIndex = 4;
            ChatInput2.TextChanged += ChatInput2_TextChanged;
            // 
            // button3
            // 
            button3.BackColor = Color.FromArgb(0, 192, 0);
            button3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button3.Location = new Point(683, 355);
            button3.Name = "button3";
            button3.Size = new Size(79, 34);
            button3.TabIndex = 5;
            button3.Text = "Send";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.BackColor = SystemColors.ActiveCaptionText;
            button4.Font = new Font("Arial", 16F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            button4.ForeColor = SystemColors.ButtonHighlight;
            button4.Location = new Point(287, 17);
            button4.Name = "button4";
            button4.Size = new Size(278, 46);
            button4.TabIndex = 6;
            button4.Text = "BOPE CHAT";
            button4.UseVisualStyleBackColor = false;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.BackColor = SystemColors.ActiveCaptionText;
            radioButton1.ForeColor = Color.Lime;
            radioButton1.Location = new Point(674, 17);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(88, 29);
            radioButton1.TabIndex = 7;
            radioButton1.TabStop = true;
            radioButton1.Text = "Online";
            radioButton1.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(radioButton1);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(ChatInput2);
            Controls.Add(ChatInput);
            Controls.Add(pictureBox1);
            Controls.Add(button2);
            Controls.Add(button1);
            Name = "Form1";
            Text = "ChatBot";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Button button2;
        private PictureBox pictureBox1;
        private TextBox ChatInput;
        private TextBox ChatInput2;
        private Button button3;
        private Button button4;
        private RadioButton radioButton1;
    }
}
