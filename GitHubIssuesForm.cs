using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShadowPilot
{
    public class GitHubIssuesForm : Form
    {
        private readonly ListBox issueListBox;
        private readonly TextBox issueDetailsTextBox;
        private readonly Button startButton;
        private readonly Button cancelButton;
        private readonly Label repositoryLabel;

        private IReadOnlyList<GitHubIssueSummary> issues;

        public GitHubIssueSummary SelectedIssue => issueListBox.SelectedItem as GitHubIssueSummary;

        public GitHubIssuesForm(GitHubRepositoryInfo repository, IReadOnlyList<GitHubIssueSummary> issues)
        {
            this.issues = issues ?? new List<GitHubIssueSummary>();

            Text = "ShadowPilot GitHub Issues";
            Width = 900;
            Height = 600;
            MinimumSize = new Size(720, 460);
            StartPosition = FormStartPosition.CenterParent;

            repositoryLabel = new Label
            {
                Text = repository?.WebUrl ?? "GitHub repository",
                Dock = DockStyle.Top,
                Height = 28,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 8, 0)
            };

            issueListBox = new ListBox
            {
                Dock = DockStyle.Left,
                Width = 360,
                IntegralHeight = false
            };
            issueListBox.SelectedIndexChanged += IssueListBox_SelectedIndexChanged;
            issueListBox.DoubleClick += IssueListBox_DoubleClick;

            issueDetailsTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font(FontFamily.GenericMonospace, 9.0f)
            };

            startButton = new Button
            {
                Text = "Start Workflow",
                DialogResult = DialogResult.OK,
                Enabled = false,
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Width = 120,
                Height = 28,
                Left = 632,
                Top = 8
            };

            cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Width = 90,
                Height = 28,
                Left = 760,
                Top = 8
            };

            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 44
            };
            buttonPanel.Controls.Add(startButton);
            buttonPanel.Controls.Add(cancelButton);
            buttonPanel.Resize += ButtonPanel_Resize;

            Controls.Add(issueDetailsTextBox);
            Controls.Add(issueListBox);
            Controls.Add(buttonPanel);
            Controls.Add(repositoryLabel);

            AcceptButton = startButton;
            CancelButton = cancelButton;

            LoadIssues();
        }

        private void LoadIssues()
        {
            issueListBox.Items.Clear();

            foreach (var issue in issues.OrderByDescending(issue => issue.UpdatedAt))
            {
                issueListBox.Items.Add(issue);
            }

            if (issueListBox.Items.Count > 0)
            {
                issueListBox.SelectedIndex = 0;
            }
            else
            {
                issueDetailsTextBox.Text = "No open issues were found for this repository.";
            }
        }

        private void IssueListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var issue = SelectedIssue;
            startButton.Enabled = issue != null;
            issueDetailsTextBox.Text = issue == null ? string.Empty : FormatIssueDetails(issue);
        }

        private void IssueListBox_DoubleClick(object sender, EventArgs e)
        {
            if (SelectedIssue != null)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void ButtonPanel_Resize(object sender, EventArgs e)
        {
            var panel = (Panel)sender;
            cancelButton.Left = panel.Width - cancelButton.Width - 16;
            startButton.Left = cancelButton.Left - startButton.Width - 8;
            cancelButton.Top = 8;
            startButton.Top = 8;
        }

        private static string FormatIssueDetails(GitHubIssueSummary issue)
        {
            var labels = issue.Labels != null && issue.Labels.Count > 0 ? string.Join(", ", issue.Labels) : "none";
            return $"#{issue.Number} {issue.Title}{Environment.NewLine}" +
                   $"State: {issue.State}{Environment.NewLine}" +
                   $"Labels: {labels}{Environment.NewLine}" +
                   $"Updated: {issue.UpdatedAt}{Environment.NewLine}" +
                   $"URL: {issue.HtmlUrl}{Environment.NewLine}{Environment.NewLine}" +
                   (string.IsNullOrWhiteSpace(issue.Body) ? "No description provided." : issue.Body);
        }
    }
}