using IMS_Services.Entities;
using IMS_Services.Services.Implementation;
using InventoryManagementSystem.Utils;
using InventoryManagementSystem.Validation;


namespace InventoryManagementSystem.Controller.InventoryFormController.InventorySubForm
{
    public partial class UpdateInventoryForm : Form
    {
        private static UpdateInventoryForm instance = null!;
        private Control[] controls;

        public static Inventory inventory { get; set; } = null!;

        public UpdateInventoryForm()
        {
            InitializeComponent();

            LoadComboBoxes.LoadProductCBO(cboProduct);
            LoadComboBoxes.LoadImportID(cboImportID);

            controls = new Control[]
            {
                txtId,
                cboImportID,
                rtxtNote,
                txtUnitCost,
                nudInitQty,
                nudCurrStock,
                dtExpireDate,
                cboProduct,
            };

            LoadEntities.LoadInvnetoryFromObject(controls, inventory);

            btnSubmit.Click += DoClickSubmit;
        }

        private void DoClickSubmit(object? sender, EventArgs e)
        {
            try
            {
                if (IsInventoryValid())
                {
                    Inventory inventory = CreatorEntities.CreateInventory(controls.Skip(2).ToArray());

                    inventory.ID = int.Parse(txtId.Text);

                    inventory.ImportID = int.Parse(cboImportID.SelectedItem.ToString());

                    bool effected = InventoryServices.Update(inventory);

                    if (effected)
                    {
                        MessageBox.Show($"Inventory ID : {txtId.Text} Info Updated!", "Updating", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
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


        private bool IsInventoryValid()
        {
            bool isVilid = true;
            isVilid = Validator.IsValidData(controls.Skip(3).ToArray());
            return isVilid;
        }

        #endregion



        #region SingleTon For Only Show One Form
        public static UpdateInventoryForm GetInstance()
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new UpdateInventoryForm();
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
}
