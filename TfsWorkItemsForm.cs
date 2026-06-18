using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShadowPilot
{
    public class TfsWorkItemsForm : Form
    {
        private readonly ListBox workItemListBox;
        private readonly TextBox detailsTextBox;
        private readonly Button closeButton;
        private readonly Label headerLabel;
        private readonly IReadOnlyList<TfsWorkItemSummary> workItems;

        public TfsWorkItemsForm(TfsWorkItemSettings settings, IReadOnlyList<TfsWorkItemSummary> workItems)
        {
            this.workItems = workItems ?? new List<TfsWorkItemSummary>();

            Text = "ShadowPilot TFS Work Items";
            Width = 900;
            Height = 600;
            MinimumSize = new Size(720, 460);
            StartPosition = FormStartPosition.CenterParent;

            headerLabel = new Label
            {
                Text = $"{settings?.Project ?? "TFS"} - assigned to {settings?.AssignedTo ?? "current user"}",
                Dock = DockStyle.Top,
                Height = 28,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 8, 0)
            };

            workItemListBox = new ListBox
            {
                Dock = DockStyle.Left,
                Width = 380,
                IntegralHeight = false
            };
            workItemListBox.SelectedIndexChanged += WorkItemListBox_SelectedIndexChanged;

            detailsTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font(FontFamily.GenericMonospace, 9.0f)
            };

            closeButton = new Button
            {
                Text = "Close",
                DialogResult = DialogResult.OK,
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Width = 90,
                Height = 28
            };

            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 44
            };
            buttonPanel.Controls.Add(closeButton);
            buttonPanel.Resize += ButtonPanel_Resize;

            Controls.Add(detailsTextBox);
            Controls.Add(workItemListBox);
            Controls.Add(buttonPanel);
            Controls.Add(headerLabel);

            AcceptButton = closeButton;
            LoadWorkItems();
        }

        private void LoadWorkItems()
        {
            workItemListBox.Items.Clear();

            foreach (var workItem in workItems.OrderBy(workItem => workItem.Id))
            {
                workItemListBox.Items.Add(workItem);
            }

            if (workItemListBox.Items.Count > 0)
            {
                workItemListBox.SelectedIndex = 0;
            }
            else
            {
                detailsTextBox.Text = "No assigned TFS work items were found.";
            }
        }

        private void WorkItemListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            detailsTextBox.Text = workItemListBox.SelectedItem is TfsWorkItemSummary workItem ? FormatWorkItem(workItem) : string.Empty;
        }

        private void ButtonPanel_Resize(object sender, EventArgs e)
        {
            var panel = (Panel)sender;
            closeButton.Left = panel.Width - closeButton.Width - 16;
            closeButton.Top = 8;
        }

        private static string FormatWorkItem(TfsWorkItemSummary workItem)
        {
            return $"#{workItem.Id} {workItem.Title}{Environment.NewLine}" +
                   $"Type: {workItem.WorkItemType}{Environment.NewLine}" +
                   $"State: {workItem.State}{Environment.NewLine}" +
                   $"Assigned To: {workItem.AssignedTo}{Environment.NewLine}" +
                   $"URL: {workItem.Url}";
        }
    }
}