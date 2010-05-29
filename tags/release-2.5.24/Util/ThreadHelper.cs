﻿using System;
using System.Windows.Forms;

namespace wyUpdate.Common
{
    public enum ProgressStatus { None, Success, Failure, SharingViolation }

    public static class ThreadHelper
    {
        public static void ReportError(ContainerControl sender, Delegate del, string errorText, Exception ex)
        {
            /*
             *
             * The reason for the do...while and the try...catch is that when an error
             * occurrs very quickly, and the windows is locked (say for repainting efficiency)
             * the .BeginInvoke will fail. Thus, I should keep retrying until it eventually succeeds.
             * 
            */

            do
            {
                try
                {
                    //Try to send our error to the frmMain thread - wait until it succeeds

                    // NOTE: a -1 for progress assures that the progress bar won't be reset

                    // eat any messages after the sender closes (aka IsDisposed)
                    if (sender.IsDisposed)
                        return;

                    sender.BeginInvoke(del, new object[] { -1, -1, errorText, ProgressStatus.Failure, ex });
                    break;
                }
                catch { }

            } while (true);
        }

        public static void ReportProgress(ContainerControl sender, Delegate del, string text, int progress, int unweightedProgress)
        {
            try
            {
                // eat any messages after the sender closes (aka IsDisposed)
                if (sender.IsDisposed)
                    return;

                sender.BeginInvoke(del, new object[] { progress, unweightedProgress, text, ProgressStatus.None, null });
            }
            catch
            {
                // don't bother with the exception (it doesn't matter if the main window misses a progress report)
            }
        }

        public static void ReportSharingViolation(ContainerControl sender, Delegate del, string filename)
        {
            try
            {
                // eat any messages after the sender closes (aka IsDisposed)
                if (sender.IsDisposed)
                    return;

                sender.BeginInvoke(del, new object[] { -1, -1, string.Empty, ProgressStatus.SharingViolation, filename });
            }
            catch
            {
                // don't bother with the exception (it doesn't matter if the main window misses a progress report)
            }
        }

        public static void ReportSuccess(ContainerControl sender, Delegate del, string text)
        {
            do
            {
                try
                {
                    // eat any messages after the sender closes (aka IsDisposed)
                    if (sender.IsDisposed)
                        return;

                    //Try to send our success to the frmMain thread - wait until it gets through

                    // NOTE: a -1 for progress assures that the progress bar won't be reset

                    sender.BeginInvoke(del, new object[] { -1, -1, text, ProgressStatus.Success, null });
                    break;
                }
                catch { }

            } while (true);
        }

        public static void ChangeRollback(ContainerControl sender, Delegate del, bool rbRegistry)
        {
            do
            {
                try
                {
                    // eat any messages after the sender closes (aka IsDisposed)
                    if (sender.IsDisposed)
                        return;

                    //Try to send our changing status to rolling back

                    sender.BeginInvoke(del, new object[] { rbRegistry });
                    break;
                }
                catch { }

            } while (true);
        }
    }
}
