using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MyComponents;

namespace TestApp
{
    public partial class Form1 : Form
    {
        private ComboBoxControl19 comboBoxControl = null!;
        private EmailInputControl19 emailInputControl = null!;
        private ListBoxControl19 listBoxControl = null!;
        private Button btnCheckEmail = null!;

        public Form1()
        {
            InitializeComponent();   
            BuildUi();                  
            LoadData();                 
        }

        private void BuildUi()
        {
            Text = "Тест компонентов варианта 19";
            Width = 640; Height = 420;

            comboBoxControl = new ComboBoxControl19
            {
                Left = 20,
                Top = 20,
                Width = 220,
                Height = 30
            };

            comboBoxControl.SelectedValueChanged += (s, e) => MessageBox.Show($"Выбрано: {comboBoxControl.SelectedValue}");

            emailInputControl = new EmailInputControl19
            {
                Left = 20,
                Top = 70,
                Width = 220,
                Height = 25
            };

            emailInputControl.SetExample("user@example.com");

            btnCheckEmail = new Button
            {
                Left = 260,
                Top = 70,
                Width = 140,
                Height = 25,
                Text = "Проверить e-mail"
            };

            btnCheckEmail.Click += (s, e) =>
            {
                try
                {
                    var email = emailInputControl.Value;
                    MessageBox.Show($"OK: {email}");
                }
                catch (EmailValidationException ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            };

            listBoxControl = new ListBoxControl19
            {
                Left = 20,
                Top = 120,
                Width = 400,
                Height = 200
            };
            listBoxControl.SetTemplate("ФИО: {Name}, Возраст: {Age}");


            Controls.AddRange(new Control[] { comboBoxControl, emailInputControl, btnCheckEmail, listBoxControl });

        }

        private void LoadData()
        {
            comboBoxControl.FillItems(new[] { "Значение 1", "Значение 2", "Значение 3" });

            var people = new List<Person>
            {
                new Person { Name = "Иван Иванов", Age = 25 },
                new Person { Name = "Пётр Петров", Age = 30 },
                new Person { Name = "Сергей Сергеев", Age = 40 }
            };
            listBoxControl.FillItems(people);

            listBoxControl.DoubleClick += (s, e) =>
            {
                var p = listBoxControl.GetSelectedObject<Person>();
                if (p != null) MessageBox.Show($"{p.Name}, {p.Age} лет");
            };

        }
    }

    public class Person
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
    }
}
