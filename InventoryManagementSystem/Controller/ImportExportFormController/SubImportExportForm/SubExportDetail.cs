﻿using IMS_Services.Entities;
using IMS_Services.Services.Implementation;
using InventoryManagementSystem.Utils;
using InventoryManagementSystem.Validation;
using System.Data;
using System.Windows.Forms;

namespace InventoryManagementSystem.Controller.ImportExportFormController.SubImportExportForm
{
    public partial class SubExportDetail : Form
    {
        public event ExportDetailHandler? ExportModified;


        private static SubExportDetail instance = null!;
        private Control[] controls;
        private BindingSource bs = new BindingSource();
        public static DataTable Table = new DataTable(ExportDetailServices.EXPD_TB_NAME);
        public SubExportDetail()
        {
            InitializeComponent();
            Table.Rows.Clear();
            bs.DataSource = Table;
            dgvExportDetail.DataSource = bs;

            LoadComboBoxes.LoadProductCBO(cboProductId);
            LoadComboBoxes.LoadExportID(cboExportID);
            ConfigView();

            controls = new Control[]
            {   
                cboExportID,
                txtUnitPrice,
                nudQuantityExport,
                txtSubTotal,
                cboProductId,
                cboInvID,
            };

            cboProductId.SelectedIndexChanged += DoProductIndexChanged;
            btnAdd.Click += DoClickAdd;
            btnDelete.Click += DoClickDelete;
            btnSubmit.Click += DoClickSubmit;
        }

        private void DoClickDelete(object? sender, EventArgs e)
        {
            if (dgvExportDetail.CurrentRow == null) return;

            DataRowView rowView = (DataRowView)dgvExportDetail.CurrentRow.DataBoundItem;
            DataRow row = rowView.Row;

            Table.Rows.Remove(row);

        }

        private void DoClickSubmit(object? sender, EventArgs e)
        {
            if (cboExportID.SelectedItem == null)
            {
                MessageBox.Show("Please choose an export ID first!", "Batch Submitting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboExportID.Focus();
                return;
            }
            try
            {
                //Add import id to the table
                foreach (DataRow row in Table.Rows)
                {
                    row[ExportDetailServices.EXPD_COL_EXPID] = cboExportID.SelectedItem;
                }
                ExportDetailServices.Submit(Table.TableName).WriteToServer(Table);

                MessageBox.Show("Sucessfully Submitted", "Submitting", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Table.Rows.Clear();
                ExportModified?.Invoke(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Submitting", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DoClickAdd(object? sender, EventArgs e)
        {
            try
            {
                if (IsInventoryValid())
                {
                    string[] arr = cboInvID.Text.Split('-');
                    int qtyStock = int.Parse(arr[1]);
                    int invID = int.Parse(arr[0]);
                    if (nudQuantityExport.Value > qtyStock)
                    {
                        MessageBox.Show("Not enough quantity on this inventory!", "Adding Row", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    
                    DataRow[] foundRows = Table.Select($"InvId = {invID}");
                    int sum = 0;
                    foreach (DataRow row in foundRows)
                    {
                        sum += int.Parse(row["QtyExported"].ToString()!); 
                    }
                    if (sum + nudQuantityExport.Value > qtyStock)
                    {
                        MessageBox.Show("Not enough quantity on this inventory!", "Adding Row", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }


                    ExportDetail expDetail = CreatorEntities.CreateExportDetail(controls.Skip(1).ToArray());

                    Table.Rows.Add(ExportDetailServices.AddRow(Table, expDetail));

                    bs.ResetBindings(true);

                    Util.ClearControls(controls);

                    MessageBox.Show($"Product Added!", "Creating", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else
                {
                    MessageBox.Show("Some Inputs are missing or not correct format!", "Creating", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DoProductIndexChanged(object? sender, EventArgs e)
        {
            if (cboProductId.SelectedItem == null) return;
            LoadComboBoxes.LoadInventoryID(cboInvID, (int)cboProductId.SelectedValue!);
        }


        #region SingleTon For Only Show One Form
        public static SubExportDetail GetInstance()
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new SubExportDetail();
            }
            return instance;
        }
        public new void Show()
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = this;
                base.Show();
            }
            else
            {
                // Bring the existing form to the front if already open
                instance.BringToFront();
            }
        }
        #endregion


        #region Validation

        private bool IsInventoryValid()
        {
            bool isVilid = true;
            isVilid = Validator.IsValidData(controls.Skip(2).ToArray());
            return isVilid;
        }

        #endregion

        private void ConfigView()
        {
   

            dgvExportDetail.Columns[ExportDetailServices.EXPD_COL_ID].Visible = false;
            dgvExportDetail.Columns[ExportDetailServices.EXPD_COL_EXPID].Visible = false;


            dgvExportDetail.Columns[InventoryServices.INV_COL_ID].HeaderText = "ID";
            dgvExportDetail.Columns[ExportDetailServices.EXPD_COL_QTYEXP].Width = 140;
            dgvExportDetail.Columns[ExportDetailServices.EXPD_COL_UNITPRICE].Width = 140;
            dgvExportDetail.Columns[ExportDetailServices.EXPD_COL_STOTAL].Width = 140;
            dgvExportDetail.Columns[ExportDetailServices.EXPD_COL_PROID].Width = 140;
            dgvExportDetail.Columns[ExportDetailServices.EXPD_COL_INVID].Width = 140;
            

            dgvExportDetail.Columns[InventoryServices.INV_COL_PRO_ID].DisplayIndex = 0;

        }
    }
}
