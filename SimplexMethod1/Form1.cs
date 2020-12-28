using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace SimplexMethod1
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}
		//Общие переменные
		int UnKnownVars = 0; //Начальные неизвестные в главной функции
		int UnKnownVarsAdd = 0; //Число дополнительных паременных (надо добавить)
		string OptType = null;
		bool IsDecisionAComplete = false;

		private void textBox1_TextChanged(object sender, EventArgs e) //Вывод значений
		{

		}

		private void button1_Click(object sender, EventArgs e) //Кнопка импорта данных
		{
			openFileDialog1.Filter = "Текстовой файл(*.txt)|*.txt";
			if (openFileDialog1.ShowDialog() == DialogResult.Cancel) return;
			else
			{
				InitializeInputData(); //Определяем тип данных (max/min), число целевых переменных и число новых добавляемых переменных (по одной на каждое уравнение-ограничение)
				//LoadInputDataToMatrix();
			}
		}

		private void openFileDialog1_FileOk(object sender, CancelEventArgs e) //Функция открытия 
		{

		}
		private void InitializeInputData ()
		{
			string DataPath = openFileDialog1.FileName; //@"C:\Work\Code\MathModelling_SPbSTU\ExampleData\test1.txt"
			//string DataPath = @"C:\Work\Code\MathModelling_SPbSTU\ExampleData\test1.txt";
			int DataileLength = 0;
			foreach (string Str in File.ReadLines(DataPath))
			{
				DataileLength++;
			}
			UnKnownVarsAdd = DataileLength - 1; //За минусом первой строки

			string[] DataFile = File.ReadAllLines(DataPath); //Получаем массив строк исходного файла

			UnKnownVars = DataFile[0].Split(';').Length - 1; //Получаем число неизвестных
			OptType = DataFile[0].Split(';').Last(); //Получаем тим функции оптимизации
			UnKnownVarsAdd = DataFile.Length - 1; //Получаем число дополнительных переменных (для сведения к каноническому виду) за минусом первой строки

			dataGridView1.ColumnCount = UnKnownVarsAdd + UnKnownVars +2; //Присваиваем таблице число колонок по числу всех переменных + колонка с Х0 и коэф. с
			dataGridView1.RowCount = UnKnownVarsAdd + 2; //Присваиваем таблице число строк по числу всех дополнительных переменных (уравнения ограничений) 
																									 //+ коэф. переменных из целевого уравнения + строку с двойственной оценкой
			
			//Задаем ширину ячеек таблицы и выравивание по центру
			for (int i = 0; i < dataGridView1.RowCount; i++)
			{
				for (int j = 0; j < dataGridView1.ColumnCount; j++)
				{
					dataGridView1.Columns[j].Width = 50;
				}
			}

			for (int j1 = 2; j1 <= UnKnownVars + 2; j1++)//Заполнение первой постоянной строки (с коэффициентами при главном уравнении оптимизации) для известных переменных
			{
				dataGridView1[j1, 0].Value = $"cj{j1-1} = " + DataFile[0].Split(';')[j1 - 2]; 
			}
			for (int j2 = 2+ UnKnownVars; j2 < 2 + UnKnownVars + UnKnownVarsAdd; j2++)//Заполнение первой постоянной строки (с коэффициентами при главном уравнении оптимизации) для доп. переменных
			{
				dataGridView1[j2, 0].Value = $"cj{j2 - 1} = " + 0; 
			}
			dataGridView1[0, 0].Value = "cσ";
			for (int i3 = 1; i3 < dataGridView1.RowCount-1;i3++) //Заполнение первого столбца нулевыми коэффициентами при доп. переменных
			{
				dataGridView1[0, i3].Value = 0;
			}
			dataGridView1[1, 0].Value = "X0";
			for (int i4 = 1; i4 < dataGridView1.RowCount - 1; i4++) //Заполнение второго столбца значениями bi
			{
				dataGridView1[1, i4].Value = DataFile[i4].Split(';').Last();
			}
			dataGridView1[1, dataGridView1.RowCount-1].Value = 0; //Текущее значение целевой функции = 0

			//Заполняем ячейки значениями коэффициентов из уравнений ограничений
			for (int i6 = 2; i6 <= UnKnownVars+2; i6++)
			{
				for (int i7 = 1; i7 < dataGridView1.RowCount-1; i7++)
				{
					dataGridView1[i6, i7].Value = DataFile[i7].Split(';')[i6 - 2];
				}
			}
			//Присваиваем коэффициенты дополнительным переменным
			for (int i6 = 2 + UnKnownVars; i6 < UnKnownVarsAdd + UnKnownVars + 2; i6++)
			{
				for (int i7 = 1; i7 < dataGridView1.RowCount - 1; i7++)
				{
					int temp2 = UnKnownVars;
					int Koef = 0;

					if ((i6 - 2 - temp2) == (i7 - 1)) Koef = 1;
					dataGridView1[i6, i7].Value = Koef;
					temp2++;
				}
			}

			CalcDualEvaluation(); //Запуск расчета двойственных оценок
		}
		private void CalcDualEvaluation () //Запуск расчета двойственных оценок
		{
			//StringBuilder temp1 = new StringBuilder();
			double DualEvaluation = 0d;
			for (int i5 = 2; i5 < dataGridView1.ColumnCount; i5++)
			{
				for (int i6 = 1; i6 < dataGridView1.RowCount; i6++) //Считыванием строковые переменные кроме первой строки (коэф) и последней (для двойств. оценок)
				{
					DualEvaluation = DualEvaluation + Convert.ToDouble(dataGridView1[i5, i6].Value) * Convert.ToDouble(dataGridView1[0, i6].Value);
				}
				dataGridView1[i5, dataGridView1.RowCount-1].Value = (DualEvaluation - Convert.ToDouble(dataGridView1[i5, 0].Value.ToString().Split('=')[1]));
				DualEvaluation = 0d;

			}

		}
		private void CheckDualEvaluations ()
		{
			for (int i = 2; i < dataGridView1.ColumnCount; i++)
			{
				if (Convert.ToDouble(dataGridView1[i, dataGridView1.RowCount - 1].Value) < 0)
				{
					IsDecisionAComplete = false;
					break;
				}
				else IsDecisionAComplete = true;
			}
			if (IsDecisionAComplete == true) MessageBox.Show("Текущее решение оптимально, пересчет не требуется!");
			else NewIteration(); // MessageBox.Show("Нужна оптимизация");

		}
		private void NewIteration()
		{
			//Шаг 1 - Определяем вектор Ak, вводимый в базис, по минимальному значению двойственной оценки - IndexOfK_J
			double TempMin = 0;
			if (OptType == "min")
			{
				TempMin = -10000000;
			}
			else TempMin = 10000000;


			int IndexOfK_J = 0;
			double dk = 0d;
			//if (OptType == "min")
			//{
			//	for (int i = 2; i < dataGridView1.ColumnCount; i++)
			//	{
			//		if (Convert.ToDouble(dataGridView1[i, dataGridView1.RowCount - 1].Value) > TempMin)
			//		{
			//			TempMin = Convert.ToDouble(dataGridView1[i, dataGridView1.RowCount - 1].Value);
			//			IndexOfK_J = i;
			//		}
			//	}
			//}

		for (int i = 2; i < dataGridView1.ColumnCount; i++)
		{
				if (Convert.ToDouble(dataGridView1[i, dataGridView1.RowCount - 1].Value) < TempMin)
				{
					TempMin = Convert.ToDouble(dataGridView1[i, dataGridView1.RowCount - 1].Value);
					IndexOfK_J = i;
				}
		}

			dk = TempMin;
			//MessageBox.Show("IndexOfK_J = " + dataGridView1[IndexOfK_J,0].Value.ToString().Split('=')[1]);
			//Шаг 2 - Определение вектора Ar, выводимого из базиса по минимальному соотношению отношения текущ. значения базисной переменной и a'rk
			int IndexOfR_I = 0;
			TempMin = 10000000;
			
			for (int i2 = 1; i2 < dataGridView1.RowCount - 1; i2++)
			{
				if ((Convert.ToDouble(dataGridView1[1, i2].Value) / Convert.ToDouble(dataGridView1[IndexOfK_J, i2].Value)) < TempMin)
				{
					TempMin = Convert.ToDouble(dataGridView1[1, i2].Value) / Convert.ToDouble(dataGridView1[IndexOfK_J, i2].Value);
					IndexOfR_I = i2;
				}
			}
			
			//MessageBox.Show("IndexOfR_I = " + IndexOfR_I);
			//Шаг 3 - пересчет элементов матрицы
			//3.1. Сохранение текущей таблицы
			string[,] CurrentMatrix = new string[dataGridView1.ColumnCount, dataGridView1.RowCount];

			for (int i = 0; i < dataGridView1.RowCount; i++)
			{
				for (int j = 0; j < dataGridView1.ColumnCount; j++)
				{
					CurrentMatrix[j, i] = Convert.ToString(dataGridView1[j, i].Value);
				}
			}
			//3.2 Пересчет элементов матрицы

			
			for (int j1 = 2; j1 < dataGridView1.ColumnCount; j1++) //Колонки
			{
				for (int i1 = 1; i1 < dataGridView1.RowCount - 1; i1++) //Строки
				{
					//Пересчет зеленой области
					if (i1 != IndexOfR_I && j1 != IndexOfK_J)
					{
						dataGridView1[j1, i1].Value = Convert.ToDouble(CurrentMatrix[j1, i1]) -
							Convert.ToDouble(CurrentMatrix[IndexOfK_J, i1]) * Convert.ToDouble(CurrentMatrix[j1, IndexOfR_I]) / Convert.ToDouble(CurrentMatrix[IndexOfK_J, IndexOfR_I]);
					}
					//Пересчет голубой области
					else if (i1 == IndexOfR_I)
					{
						dataGridView1[j1, i1].Value = Convert.ToDouble(CurrentMatrix[j1, IndexOfR_I]) / Convert.ToDouble(CurrentMatrix[IndexOfK_J, IndexOfR_I]);
					}
					//Пересчет желтой области
					else if (i1 != IndexOfR_I && j1 == IndexOfK_J)
					{
						dataGridView1[j1, i1].Value = 0;
					}
					//Пересчет желтой области в перекрестье
					else if (i1 == IndexOfR_I && j1 == IndexOfK_J)
					{
						dataGridView1[j1, i1].Value = 1;
					}
				}
			}
			//3.3 Пересчет значений базисов Х0

			for (int i1 = 1; i1 < dataGridView1.RowCount - 1; i1++) //Строки
			{

				if (i1 != IndexOfR_I)
				{
					dataGridView1[1, i1].Value = Convert.ToDouble(CurrentMatrix[1, i1]) -
						Convert.ToDouble(CurrentMatrix[IndexOfK_J, i1]) * Convert.ToDouble(CurrentMatrix[1, IndexOfR_I]) / Convert.ToDouble(CurrentMatrix[IndexOfK_J, IndexOfR_I]);
				}
				else if (i1 == IndexOfR_I)
				{
					dataGridView1[1, i1].Value = Convert.ToDouble(CurrentMatrix[1, IndexOfR_I]) / Convert.ToDouble(CurrentMatrix[IndexOfK_J, IndexOfR_I]);
				}
			}
			//3.4 Пересчет двойственных оценок
			for (int j1 = 2; j1 < dataGridView1.ColumnCount; j1++)
			{
				dataGridView1[j1, dataGridView1.RowCount - 1].Value = Convert.ToDouble(CurrentMatrix[j1, dataGridView1.RowCount - 1]) - Convert.ToDouble(CurrentMatrix[j1, IndexOfR_I]) / Convert.ToDouble(CurrentMatrix[IndexOfK_J, IndexOfR_I]) * dk;
			}
			//3.5 Пересчет значения целевой функции
			dataGridView1[1, dataGridView1.RowCount - 1].Value = Convert.ToDouble(CurrentMatrix[1, dataGridView1.RowCount - 1]) - Convert.ToDouble(CurrentMatrix[1, IndexOfR_I]) / Convert.ToDouble(CurrentMatrix[IndexOfK_J, IndexOfR_I]) * dk;
			//3.6 Пересчет коэффициентов целевой функции
			for (int i1 = 1; i1 < dataGridView1.RowCount - 1; i1++)
			{	
				if (i1 == IndexOfR_I)
				{
					dataGridView1[0, i1].Value = Convert.ToDouble(CurrentMatrix[IndexOfK_J, 0].Split('=')[1]);
				}
				
			}

		}
		private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) //Таблица для данных
		{

		}

		private void button2_Click(object sender, EventArgs e) //Кнопка запуска итерации расчета
		{
			CheckDualEvaluations();
		}
	}
}
