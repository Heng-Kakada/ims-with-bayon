﻿using IMS_Services.Entities;
using IMS_Services.Services.Implementation;
using InventoryManagementSystem.Controller.InventoryFormController.InventorySubForm;


namespace InventoryManagementSystem.Controller
{
    public partial class ProductForm : Form
    {
        public ProductForm()
        {
            InitializeComponent();
            LoadData();

            btnAdd.Click += DoClickAddProduct;
            btnUpdate.Click += DoClickUpdateProduct;
            btnDelete.Click += DoClickDeleteProduct;
            
            dgvProduct.Click += DoClickDataGridView;

        }

        #region Event
        private void DoClickDeleteProduct(object? sender, EventArgs e)
        {
            try
            {
                int id = (int)dgvProduct.CurrentRow.Cells[0].Value;

                if (id.Equals(-1))
                {
                    MessageBox.Show("Please Choose Staff You Want To Delete First ...", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show($"Are you sure you want to delete the Product member with ID {id}?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    bool isDeleted = ProductServices.Delete(id);

                    if (isDeleted)
                    {
                        MessageBox.Show($"Product with ID {id} has been successfully deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"No Product member found with ID {id}.","Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Deletion canceled.","Canceled",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }

                LoadData();                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DoClickDataGridView(object? sender, EventArgs e)
        {
            if (dgvProduct.CurrentRow == null) return;
        }

        private void DoClickUpdateProduct(object? sender, EventArgs e)
        {
            if (dgvProduct.CurrentRow == null)
            {
                MessageBox.Show("Please pick a product!", "Updating", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Product product = ProductServices.GetById((int)dgvProduct.CurrentRow.Cells[0].Value);

            SubProduct.product = product;

            SubProduct subProduct = new SubProduct();
            
            subProduct.ProductModified += (sender) => LoadData();

            subProduct.Show();
        }

        private void DoClickAddProduct(object? sender, EventArgs e)
        {
            SubProduct subProduct = new SubProduct();
            subProduct.ProductModified += (sender) => LoadData();
            subProduct.Show();
        }
        #endregion


        #region method

        private void LoadData()
        {
            dgvProduct.Rows.Clear();
            var result = ProductServices.GetAll();
            foreach (Product product in result)
            {
                AddToView(product);
            }
        }
        private void AddToView(Product product)
        {
            dgvProduct.Rows.Add(product.ID, product.Barcode, product.Name, product.SalePrice, product.UOM, product.TotalStock, product.CategoryID);
        }

        #endregion
    }
}
