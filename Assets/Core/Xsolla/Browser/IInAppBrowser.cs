using System;

namespace Xsolla.Core
{
    public interface IInAppBrowser
    {
        bool IsOpened { get; }

        bool IsFullScreen { get; }
        event Action OpenEvent;

        event Action<BrowserCloseInfo> CloseEvent;

        event Action<string> UrlChangeEvent;

        event Action<string, Action> AlertDialogEvent;

        event Action<string, Action, Action> ConfirmDialogEvent;

        void Open(string url);

        void Close(float delay = 0f, bool isManually = false);

        void AddInitHandler(Action callback);

        void AddCloseHandler(Action callback);

        void AddUrlChangeHandler(Action<string> callback);

        void UpdateSize(int width, int height);

        void SetFullscreenMode(bool isFullscreen);
    }
}