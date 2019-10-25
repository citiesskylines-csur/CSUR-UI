using ColossalFramework.UI;
using CSUR_UI;
using CSUR_UI.Util;
using UnityEngine;

namespace CSUR_UI.UI
{
    public class MainButton : UIButton
    {
        private UIComponent MainUITrigger_paneltime;
        private UIComponent MainUITrigger_chirper;
        private UIComponent MainUITrigger_esc;
        private UIComponent MainUITrigger_infopanel;
        private UIComponent MainUITrigger_bottombars;
        private UIDragHandle m_DragHandler;

        public static void MainUIToggle()
        {
            if (!Loader.mainUI.isVisible)
            {
                MainUI.refeshOnce = true;
                Loader.mainUI.Show();
            }
            else
            {
                Loader.mainUI.Hide();
            }
        }

        public void MainUIOff()
        {
            if (Loader.mainUI.isVisible && !Loader.mainUI.containsMouse && !containsMouse && MainUITrigger_paneltime != null && !MainUITrigger_paneltime.containsMouse)
            {
                Loader.mainUI.Hide();
            }
        }

        public override void Start()
        {
            name = "MainButton";
            relativePosition = new Vector3((Loader.parentGuiView.fixedWidth - 50f), (Loader.parentGuiView.fixedHeight / 2 + 150f));
            normalBgSprite = "ToolbarIconGroup1Nomarl";
            hoveredBgSprite = "ToolbarIconGroup1Hovered";
            focusedBgSprite = "ToolbarIconGroup1Focused";
            pressedBgSprite = "ToolbarIconGroup1Pressed";
            playAudioEvents = true;
            text = "CSUR";
            //UISprite internalSprite = AddUIComponent<UISprite>();
            //internalSprite.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            //internalSprite.spriteName = "RcButton";
            //internalSprite.relativePosition = new Vector3(0, 0);
            //internalSprite.width = 50f;
            //internalSprite.height = 50f;
            size = new Vector2(50f, 50f);
            zOrder = 11;
            m_DragHandler = AddUIComponent<UIDragHandle>();
            m_DragHandler.target = this;
            m_DragHandler.relativePosition = Vector2.zero;
            m_DragHandler.width = 50;
            m_DragHandler.height = 50;
            m_DragHandler.zOrder = 10;
            m_DragHandler.Start();
            m_DragHandler.enabled = true;
            eventDoubleClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                MainUIToggle();
            };
            MainUITrigger_chirper = UIView.Find<UIPanel>("ChirperPanel");
            MainUITrigger_esc = UIView.Find<UIButton>("Esc");
            MainUITrigger_infopanel = UIView.Find<UIPanel>("InfoPanel");
            MainUITrigger_bottombars = UIView.Find<UISlicedSprite>("TSBar");
            MainUITrigger_paneltime = UIView.Find<UIPanel>("PanelTime");
            if (MainUITrigger_chirper != null && MainUITrigger_paneltime != null)
            {
                MainUITrigger_chirper.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
                {
                    MainUIOff();
                };
            }
            if (MainUITrigger_esc != null && MainUITrigger_paneltime != null)
            {
                MainUITrigger_esc.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
                {
                    MainUIOff();
                };
            }
            if (MainUITrigger_infopanel != null && MainUITrigger_paneltime != null)
            {
                MainUITrigger_infopanel.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
                {
                    MainUIOff();
                };
            }
            if (MainUITrigger_bottombars != null && MainUITrigger_paneltime != null)
            {
                MainUITrigger_bottombars.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
                {
                    MainUIOff();
                };
            }
        }

        public override void Update()
        {
            if (Loader.isGuiRunning)
            {
                if (Loader.mainUI.isVisible)
                {
                    Focus();
                    Hide();
                }
                else
                {
                    Unfocus();
                    Show();
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToolsModifierControl.SetTool<DefaultTool>();
            }
            base.Update();
        }
    }
}
