using IMS_Services.Entities;
using IMS_Services.Services.Implementation;
using InventoryManagementSystem.Utils;
using InventoryManagementSystem.Validation;

namespace InventoryManagementSystem.Controller.ImportExportFormController.SubImportExportForm;

public partial class UpdateExportDetail : Form
{
    public event ExportDetailHandler? ExportModified;


    private static UpdateExportDetail instance = null!;
    private Control[] controls;

    public static ExportDetail exportDetail { get; set; } = null!;
    public UpdateExportDetail()
    {
        InitializeComponent();

        LoadComboBoxes.LoadProductCBO(cboProductId);
        
        LoadComboBoxes.LoadExportID(cboExportID);
        

        controls = new Control[]
        {
                txtId,
                cboExportID,
                txtUnitPrice,
                nudQuantityExport,
                txtSubTotal,
                cboProductId,
                cboInvID,
        };

        LoadEntities.LoadExportDetailFromObject(controls, exportDetail);
        
        cboProductId.SelectedIndexChanged += DoProductIdIndexChanged;
        btnSubmit.Click += DoClickSubmit;
    }

    private void DoProductIdIndexChanged(object? sender, EventArgs e)
    {
        if (cboProductId.SelectedItem == null) return;
        LoadComboBoxes.LoadInventoryID(cboInvID, (int)cboProductId.SelectedValue!);
    }

    private void DoClickSubmit(object? sender, EventArgs e)
    {
        try
        {
            if (IsExportDetailValid())
            {
                string[] arr = cboInvID.Text.Split('-');
                int qtyStock = int.Parse(arr[1]);
                if (nudQuantityExport.Value > qtyStock)
                {
                    MessageBox.Show("Not enough quantity on this inventory!", "Adding Row", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                ExportDetail exportDetail = CreatorEntities.CreateExportDetail(controls.Skip(2).ToArray());

                exportDetail.ID = int.Parse(txtId.Text);

                exportDetail.ExportID = int.Parse(cboExportID.SelectedItem.ToString());

                bool effected = ExportDetailServices.Update(exportDetail);

                if (effected)
                {
                    MessageBox.Show($"Export Detail ID : {txtId.Text} Info Updated!", "Updating", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ExportModified?.Invoke(this);

                    Util.ClearControls(controls);
                }
                else
                {
                    MessageBox.Show($"Error Updating Data", "Updating", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


                Util.ClearControls(controls);
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

    #region Validation


    private bool IsExportDetailValid()
    {
        bool isVilid = true;
        isVilid = Validator.IsValidData(controls.Skip(1).ToArray());
        return isVilid;
    }

    #endregion

    #region SingleTon For Only Show One Form
    public static UpdateExportDetail GetInstance()
    {
        if (instance == null || instance.IsDisposed)
        {
            instance = new UpdateExportDetail();
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
}
