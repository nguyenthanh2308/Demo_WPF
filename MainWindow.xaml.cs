using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;

namespace Ontap_GK_wpf_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string st =@"Data Source=MSI\SQLEXPRESS;Initial Catalog=QLNhanSu;Integrated Security=True";
        SqlConnection cn;
        SqlDataAdapter da;
        DataSet ds;
        SqlCommandBuilder builder;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(Window_load); //Sự kiện load form

            btnThem.Click += new RoutedEventHandler(Them);
            btnSua.Click += new RoutedEventHandler(Sua);
            btnXoa.Click += new RoutedEventHandler(Xoa);
            btnLamMoi.Click += new RoutedEventHandler(LamMoi);
            btnThongKe.Click += new RoutedEventHandler(ThongKePhongBan);
            btnTimKiem.Click += new RoutedEventHandler(TimKiem);

            DataGrid.SelectionChanged += new SelectionChangedEventHandler(Data_Click);
        }

        private void Xoa(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("Bạn có muốn xoá không?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (dr == MessageBoxResult.Yes)
            {
                int i = DataGrid.SelectedIndex;//Chỉ dòng được chọn
                try
                {
                    DataTable dt = ds.Tables["NhanVien"];
                    //Xoá trên DataSet
                    dt.Rows[i].Delete();
                    //Cập nhật từ DataSet xuống Database
                    da.Update(ds, "NhanVien");
                    MessageBox.Show("Xoá thành công");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Xoá không thành công");
                }
            }
        }

        private void TimKiem(object sender, RoutedEventArgs e)
        {
            string sql = string.Format("SELECT * FROM DSNV WHERE Hoten like N'%{0}'", txtTimKiem.Text.Trim());
            da = new SqlDataAdapter(sql, cn);
            DataTable dt = new DataTable();
            da.Fill(dt);

            DataGrid.ItemsSource = dt.DefaultView;
        }

        private void ThongKePhongBan(object sender, RoutedEventArgs e)
        {
            string sql = @"SELECT B.TenPhong, count (A.MaPhong) as SoLuong " +
                "FROM DSNV as A, DMPHONG as B " +
                "WHERE A.MaPhong = B.MaPhong " +
                "GROUP BY B.TenPhong";
            da = new SqlDataAdapter(sql, cn);
            DataTable dt = new DataTable();
            da.Fill(dt);

            DataGrid.ItemsSource = dt.DefaultView;
        }

        private void LamMoi(object sender, RoutedEventArgs e)
        {
            txtMaNV.Clear();
            txtHoTen.Clear();
            txtHoTen.Focus();
            dtpNgaySinh.Text = DateTime.Now.ToString();
            if (rdNam.IsChecked == false)
                rdNam.IsChecked = true;
            txtHSL.Clear();
            txtSoDT.Clear();
            cboTenPhong.SelectedIndex = 0;
            cboTenChucVu.SelectedIndex = 0;

            DataGrid.ItemsSource = ds.Tables["NhanVien"].DefaultView;
        }

        private void Sua(object sender, RoutedEventArgs e)
        {
            DataTable dt = ds.Tables["NhanVien"];
            int ma = int.Parse(txtMaNV.Text);
            string dk = string.Format("MaNV={0}", ma);
            DataRow[] rows = dt.Select(dk);
            foreach (DataRow r in rows)
            {
                r[1] = txtHoTen.Text;
                r[2] = dtpNgaySinh.SelectedDate.ToString();
                r[3] = rdNam.IsChecked == true ? true : false;
                r[4] = txtSoDT.Text;
                r[5] = Math.Round(float.Parse(txtHSL.Text), 2);
                r[6] = cboTenPhong.SelectedValue.ToString();
                r[7] = cboTenChucVu.SelectedValue.ToString();

                //Cập nhật dữ liệu từ DataSet xuống DataBase
                da.Update(ds, "NhanVien");

                MessageBox.Show("Sửa Thành Công");
            }
        }

        private void Them(object sender, RoutedEventArgs e)
        {
            //Thêm trên DataSet
            DataTable dt = ds.Tables["NhanVien"];
            DataRow r = dt.NewRow(); // Thêm dòng trống
            r[1] = txtHoTen.Text;
            r[2] = dtpNgaySinh.SelectedDate.ToString();
            r[3] = rdNam.IsChecked == true ? true : false;
            r[4] = txtSoDT.Text;
            r[5] = Math.Round(float.Parse(txtHSL.Text), 2);
            r[6] = cboTenPhong.SelectedValue.ToString();
            r[7] = cboTenChucVu.SelectedValue.ToString();
            dt.Rows.Add(r);

            //cập nhật dữ liệu từ DataSet xuống DataBase
            da.Update(ds, "NhanVien");

            MessageBox.Show("Thêm thành công");
        }

        private void Window_load(object sender, RoutedEventArgs e)
        {
            cn = new SqlConnection(st);
            ds = new DataSet();
            loadChucVu();
            loadPB();
            loadDSNV();
            builder = new SqlCommandBuilder(da);
        }
        #region load dữ liệu
        //Phương thức load dữ liệu lên Combobox phòng ban
        public void loadPB()
        {
            string sql = "SELECT * FROM DMPHONG";
            //Khởi tạo Adapter
            da = new SqlDataAdapter(sql, cn);
            //Đổ dữ liệu lên DataSet
            da.Fill(ds, "PhongBan");

            //Lấy dữ liệu từ dataSet đổ lên Combobox
            cboTenPhong.ItemsSource = ds.Tables["PhongBan"].DefaultView;
            cboTenPhong.DisplayMemberPath = "TenPhong";
            cboTenPhong.SelectedValuePath = "MaPhong";

            da.Dispose();
        }
        //Phương thức load dữ liệu lên Combobox Chức vụ
        public void loadChucVu()
        {
            string sql = "SELECT * FROM CHUCVU";
            //Khởi tạo Adapter
            da = new SqlDataAdapter(sql, cn);
            //Đổ dữ liệu lên DataSet
            da.Fill(ds, "ChucVu");

            //Lấy dữ liệu từ DataSet đổ lên Combobox
            cboTenChucVu.ItemsSource = ds.Tables["ChucVu"].DefaultView;
            cboTenChucVu.DisplayMemberPath = "TenChucVu";
            cboTenChucVu.SelectedValuePath = "MaChucVu";

            da.Dispose();
        }    
        public void loadDSNV()
        {
            string sql = "SELECT * FROM DSNV";
            //Khởi tạo Adapter
            da = new SqlDataAdapter(sql, cn);
            //Đổ dữ liệu lên DataSet
            da.Fill(ds, "NhanVien");

            //Lấy dữ liệu từ DataSet đổ lên DataGridView
            DataGrid.ItemsSource = ds.Tables["NhanVien"].DefaultView;
            
        }

        //Kiểm tra sự trùng lặp của khoá chính
        public bool KtraMaNV (string ma)
        {
            bool kt = false;
            DataTable dt = ds.Tables["NhanVien"];
            foreach (DataRow r in dt.Rows)
                if (r[0].Equals(ma))
                {
                    kt = true;
                    break;
                }
            return kt;
        }
        private void Data_Click(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex.ToString() != null) //Có dòng được chọn
            {
                DataRowView drv = (DataRowView)DataGrid.SelectedItem;// Dòng đang chọn
                if(drv != null)
                {
                    txtMaNV.Text = drv[0].ToString();
                    txtHoTen.Text = drv[1].ToString();
                    dtpNgaySinh.Text = drv[2].ToString();
                    string gt = drv[3].ToString();
                    if (gt.Equals("True"))
                        rdNam.IsChecked = true;
                    else
                        rdNu.IsChecked = true;
                    txtSoDT.Text = drv[4].ToString();
                    txtHSL.Text = drv[5].ToString();
                    cboTenPhong.SelectedValue = drv[6].ToString();
                    cboTenChucVu.SelectedValue = drv[7].ToString();
                }
            }
        }
        #endregion
    }
}
