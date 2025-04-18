using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace wfaPaint
{
    internal class ClipboardManager
    {
        private readonly DrawingManager drawingManager;
        private readonly SelectionManager selectionManager;

        public ClipboardManager(DrawingManager drawingManager, SelectionManager selectionManager)
        {
            this.drawingManager = drawingManager;
            this.selectionManager = selectionManager;
        }

        public void CopySelectionToClipboard()
        {
            if (selectionManager.HasSelection)
            {
                using Bitmap copy = new(selectionManager.SelectedArea.Width, selectionManager.SelectedArea.Height);
                using (Graphics g = Graphics.FromImage(copy))
                {
                    g.DrawImage(drawingManager.Bitmap, new Rectangle(0, 0, copy.Width, copy.Height), selectionManager.SelectedArea, GraphicsUnit.Pixel);
                }
                Clipboard.SetImage(copy);
            }
        }

        public void PasteFromClipboard(PictureBox pictureBox)
        {
            try
            {
                IDataObject dataObject = Clipboard.GetDataObject();
                if (dataObject != null && dataObject.GetDataPresent(DataFormats.Bitmap))
                {
                    Image clipboardImage = (Image)dataObject.GetData(DataFormats.Bitmap);
                    if (clipboardImage != null)
                    {
                        drawingManager.BackupImage();

                        int insertX = (drawingManager.Bitmap.Width - clipboardImage.Width) / 2;
                        int insertY = (drawingManager.Bitmap.Height - clipboardImage.Height) / 2;
                        Point insertPoint = new Point(Math.Max(0, insertX), Math.Max(0, insertY));

                        using (Graphics g = Graphics.FromImage(drawingManager.Bitmap))
                        {
                            g.DrawImage(clipboardImage, insertPoint);
                        }

                        selectionManager.SetSelection(new Rectangle(insertPoint, clipboardImage.Size), new Bitmap(clipboardImage));
                        pictureBox.Invalidate();
                    }
                }
            }
            catch (ExternalException ex)
            {
                MessageBox.Show("Clipboard access error: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Clipboard unknown error: " + ex.Message);
            }
        }
    }
}
