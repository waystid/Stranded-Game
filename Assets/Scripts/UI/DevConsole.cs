using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.TopDownEngine;
using Unity.Cinemachine;

/// <summary>
/// In-game developer console panel.
/// Creates its own Canvas at runtime (sort order 999 — always on top).
/// Toggle with backtick (`) key or the top-right corner button.
/// </summary>
public class DevConsole : MonoBehaviour
{
    // ── Defaults ─────────────────────────────────────────────────────────────
    const float   DEF_WALK_SPEED    = 4f;
    const float   DEF_CAMERA_ZOOM   = 14f;
    const float   DEF_DAY_DURATION  = 120f;
    const float   DEF_TIME_OF_DAY   = 0.25f;
    const float   DEF_CURVATURE     = 0.002f;
    const bool    DEF_DAY_NIGHT     = true;
    const KeyCode DEF_KEY_CONSOLE   = KeyCode.BackQuote;
    const KeyCode DEF_KEY_SPRINT    = KeyCode.LeftShift;
    const KeyCode DEF_KEY_INTERACT  = KeyCode.F;
    const KeyCode DEF_KEY_JUMP      = KeyCode.Space;

    // ── Colors ────────────────────────────────────────────────────────────────
    static readonly Color ColPanel      = new Color(0.10f, 0.10f, 0.18f, 0.92f);
    static readonly Color ColPanelHead  = new Color(0.05f, 0.05f, 0.15f, 1.00f);
    static readonly Color ColHeader     = new Color(0.40f, 0.80f, 1.00f, 1.00f);
    static readonly Color ColSliderFill = new Color(0.30f, 0.70f, 1.00f, 1.00f);
    static readonly Color ColSliderBg   = new Color(0.10f, 0.10f, 0.20f, 1.00f);
    static readonly Color ColBtnNormal  = new Color(0.20f, 0.20f, 0.35f, 1.00f);
    static readonly Color ColBtnHover   = new Color(0.30f, 0.30f, 0.55f, 1.00f);
    static readonly Color ColBtnOrange  = new Color(0.90f, 0.50f, 0.10f, 1.00f);
    static readonly Color ColBtnRed     = new Color(0.50f, 0.10f, 0.10f, 1.00f);
    static readonly Color ColText       = Color.white;
    static readonly Color ColTextDim    = new Color(0.75f, 0.75f, 0.75f, 1.00f);

    // ── Runtime state ────────────────────────────────────────────────────────
    bool      _panelVisible;
    KeyCode   _toggleKey;
    KeyCode   _sprintKey;
    KeyCode   _interactKey;
    KeyCode   _jumpKey;
    string    _rebindTarget; // null = not listening
    Font      _font;

    // ── Scene references ─────────────────────────────────────────────────────
    CharacterMovement           _charMove;
    CinemachinePositionComposer _cinemachine;
    DayNightCycle               _dayNight;
    List<Material>              _curvatureMaterials = new List<Material>();

    // ── UI refs ───────────────────────────────────────────────────────────────
    CanvasGroup _panelGroup;
    Slider  _sliderWalk, _sliderZoom, _sliderDayDuration, _sliderTimeOfDay, _sliderCurvature;
    Text    _lblWalkVal, _lblZoomVal, _lblDayDurationVal, _lblTimeOfDayVal, _lblCurvatureVal;
    Toggle  _toggleDayNight;
    Button  _btnConsoleKey, _btnSprintKey, _btnInteractKey, _btnJumpKey;
    Text    _txtConsoleKey, _txtSprintKey, _txtInteractKey, _txtJumpKey;

    // ─────────────────────────────────────────────────────────────────────────
    //  AWAKE — load prefs + build UI
    // ─────────────────────────────────────────────────────────────────────────
    void Awake()
    {
        _toggleKey   = (KeyCode)PlayerPrefs.GetInt("DevConsole_Key_Console",  (int)DEF_KEY_CONSOLE);
        _sprintKey   = (KeyCode)PlayerPrefs.GetInt("DevConsole_Key_Sprint",   (int)DEF_KEY_SPRINT);
        _interactKey = (KeyCode)PlayerPrefs.GetInt("DevConsole_Key_Interact", (int)DEF_KEY_INTERACT);
        _jumpKey     = (KeyCode)PlayerPrefs.GetInt("DevConsole_Key_Jump",     (int)DEF_KEY_JUMP);

        _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (_font == null)
            _font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        BuildUI();
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  START — wire parameters to scene objects
    // ─────────────────────────────────────────────────────────────────────────
    void Start()
    {
        _charMove    = FindObjectOfType<CharacterMovement>();
        _cinemachine = FindObjectOfType<CinemachinePositionComposer>();
        _dayNight    = FindObjectOfType<DayNightCycle>();

        // Collect materials with _Curvature property
        foreach (var r in FindObjectsOfType<MeshRenderer>())
        {
            foreach (var mat in r.sharedMaterials)
            {
                if (mat != null && mat.HasProperty("_Curvature") && !_curvatureMaterials.Contains(mat))
                    _curvatureMaterials.Add(mat);
            }
        }

        // Set slider values from current scene state
        if (_charMove != null)
            _sliderWalk.SetValueWithoutNotify(_charMove.WalkSpeed);
        if (_cinemachine != null)
            _sliderZoom.SetValueWithoutNotify(_cinemachine.CameraDistance);
        if (_dayNight != null)
        {
            _sliderDayDuration.SetValueWithoutNotify(_dayNight.dayDurationSeconds);
            _sliderTimeOfDay.SetValueWithoutNotify(_dayNight.timeOfDay);
            _toggleDayNight.SetIsOnWithoutNotify(_dayNight.enabled);
        }
        if (_curvatureMaterials.Count > 0)
            _sliderCurvature.SetValueWithoutNotify(_curvatureMaterials[0].GetFloat("_Curvature"));

        // Sync value labels
        _lblWalkVal.text        = _sliderWalk.value.ToString("F1");
        _lblZoomVal.text        = _sliderZoom.value.ToString("F1");
        _lblDayDurationVal.text = _sliderDayDuration.value.ToString("F0") + "s";
        _lblTimeOfDayVal.text   = _sliderTimeOfDay.value.ToString("F2");
        _lblCurvatureVal.text   = _sliderCurvature.value.ToString("F4");

        RefreshKeyLabels();

        // Wire scene callbacks (after setting initial values)
        _sliderWalk.onValueChanged.AddListener(OnWalkSpeedChanged);
        _sliderZoom.onValueChanged.AddListener(OnCameraZoomChanged);
        _sliderDayDuration.onValueChanged.AddListener(OnDayDurationChanged);
        _sliderTimeOfDay.onValueChanged.AddListener(OnTimeOfDayChanged);
        _sliderCurvature.onValueChanged.AddListener(OnCurvatureChanged);
        _toggleDayNight.onValueChanged.AddListener(OnDayNightToggle);
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  UPDATE — toggle key + rebind capture + TDE button intercepts
    // ─────────────────────────────────────────────────────────────────────────
    void Update()
    {
        // Rebind listening mode
        if (_rebindTarget != null)
        {
            System.Array keyCodes = System.Enum.GetValues(typeof(KeyCode));
            foreach (KeyCode kc in keyCodes)
            {
                if ((int)kc >= 323 && (int)kc <= 329) continue; // skip mouse buttons
                if (kc == KeyCode.None) continue;
                if (Input.GetKeyDown(kc))
                {
                    FinishRebind(kc);
                    return;
                }
            }
            return; // don't process other actions while listening
        }

        // Toggle console panel
        if (Input.GetKeyDown(_toggleKey))
            SetPanelVisible(!_panelVisible);

        // TDE button intercepts for remapped keys
        var im = InputManager.Instance;
        if (im == null) return;

        if (Input.GetKeyDown(_sprintKey))    im.RunButtonDown();
        if (Input.GetKey(_sprintKey))        im.RunButtonPressed();
        if (Input.GetKeyUp(_sprintKey))      im.RunButtonUp();

        if (Input.GetKeyDown(_interactKey))  im.InteractButtonDown();
        if (Input.GetKeyUp(_interactKey))    im.InteractButtonUp();

        if (Input.GetKeyDown(_jumpKey))      im.JumpButtonDown();
        if (Input.GetKey(_jumpKey))          im.JumpButtonPressed();
        if (Input.GetKeyUp(_jumpKey))        im.JumpButtonUp();
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  PARAM CALLBACKS
    // ─────────────────────────────────────────────────────────────────────────
    void OnWalkSpeedChanged(float v)
    {
        _lblWalkVal.text = v.ToString("F1");
        if (_charMove == null) return;
        _charMove.WalkSpeed     = v;
        _charMove.MovementSpeed = v;
    }

    void OnCameraZoomChanged(float v)
    {
        _lblZoomVal.text = v.ToString("F1");
        if (_cinemachine == null) return;
        _cinemachine.CameraDistance = v;
    }

    void OnDayDurationChanged(float v)
    {
        _lblDayDurationVal.text = v.ToString("F0") + "s";
        if (_dayNight == null) return;
        _dayNight.dayDurationSeconds = v;
    }

    void OnTimeOfDayChanged(float v)
    {
        _lblTimeOfDayVal.text = v.ToString("F2");
        if (_dayNight == null) return;
        _dayNight.timeOfDay = v;
    }

    void OnDayNightToggle(bool val)
    {
        if (_dayNight == null) return;
        _dayNight.enabled = val;
    }

    void OnCurvatureChanged(float v)
    {
        _lblCurvatureVal.text = v.ToString("F4");
        foreach (var mat in _curvatureMaterials)
            mat.SetFloat("_Curvature", v);
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  KEY REBINDING
    // ─────────────────────────────────────────────────────────────────────────
    void StartRebind(string target)
    {
        _rebindTarget = target;
        SetRebindButtonColor(target, ColBtnOrange);
    }

    void FinishRebind(KeyCode kc)
    {
        switch (_rebindTarget)
        {
            case "Console":
                _toggleKey = kc;
                PlayerPrefs.SetInt("DevConsole_Key_Console", (int)kc);
                break;
            case "Sprint":
                _sprintKey = kc;
                PlayerPrefs.SetInt("DevConsole_Key_Sprint", (int)kc);
                break;
            case "Interact":
                _interactKey = kc;
                PlayerPrefs.SetInt("DevConsole_Key_Interact", (int)kc);
                break;
            case "Jump":
                _jumpKey = kc;
                PlayerPrefs.SetInt("DevConsole_Key_Jump", (int)kc);
                break;
        }
        PlayerPrefs.Save();
        SetRebindButtonColor(_rebindTarget, ColBtnNormal);
        _rebindTarget = null;
        RefreshKeyLabels();
    }

    void SetRebindButtonColor(string target, Color c)
    {
        Button btn = target switch
        {
            "Console"  => _btnConsoleKey,
            "Sprint"   => _btnSprintKey,
            "Interact" => _btnInteractKey,
            "Jump"     => _btnJumpKey,
            _          => null
        };
        if (btn == null) return;
        var img = btn.GetComponent<Image>();
        if (img != null) img.color = c;
    }

    void RefreshKeyLabels()
    {
        if (_txtConsoleKey  != null) _txtConsoleKey.text  = _toggleKey.ToString();
        if (_txtSprintKey   != null) _txtSprintKey.text   = _sprintKey.ToString();
        if (_txtInteractKey != null) _txtInteractKey.text = _interactKey.ToString();
        if (_txtJumpKey     != null) _txtJumpKey.text     = _jumpKey.ToString();
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  RESET DEFAULTS
    // ─────────────────────────────────────────────────────────────────────────
    void ResetDefaults()
    {
        _sliderWalk.value        = DEF_WALK_SPEED;
        _sliderZoom.value        = DEF_CAMERA_ZOOM;
        _sliderDayDuration.value = DEF_DAY_DURATION;
        _sliderTimeOfDay.value   = DEF_TIME_OF_DAY;
        _sliderCurvature.value   = DEF_CURVATURE;
        _toggleDayNight.isOn     = DEF_DAY_NIGHT;

        _toggleKey   = DEF_KEY_CONSOLE;
        _sprintKey   = DEF_KEY_SPRINT;
        _interactKey = DEF_KEY_INTERACT;
        _jumpKey     = DEF_KEY_JUMP;

        PlayerPrefs.DeleteKey("DevConsole_Key_Console");
        PlayerPrefs.DeleteKey("DevConsole_Key_Sprint");
        PlayerPrefs.DeleteKey("DevConsole_Key_Interact");
        PlayerPrefs.DeleteKey("DevConsole_Key_Jump");
        PlayerPrefs.Save();

        RefreshKeyLabels();
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  PANEL VISIBILITY
    // ─────────────────────────────────────────────────────────────────────────
    void SetPanelVisible(bool visible)
    {
        _panelVisible              = visible;
        _panelGroup.alpha          = visible ? 1f : 0f;
        _panelGroup.blocksRaycasts = visible;
        _panelGroup.interactable   = visible;
    }

    // =========================================================================
    //  UI BUILDER
    // =========================================================================
    void BuildUI()
    {
        // ── Root Canvas (Screen Space Overlay, always on top, survives scene loads)
        var canvasGO = new GameObject("DevConsole_Canvas");
        DontDestroyOnLoad(canvasGO);

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode     = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight  = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // Only create EventSystem if one doesn't already exist
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var esGO = new GameObject("EventSystem");
            esGO.transform.SetParent(canvasGO.transform, false);
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // ── Toggle button (top-right corner, always visible regardless of panel)
        var toggleBtnGO = MakeRect("ToggleBtn", canvasGO.transform);
        var tRect = toggleBtnGO.GetComponent<RectTransform>();
        tRect.anchorMin        = Vector2.one;
        tRect.anchorMax        = Vector2.one;
        tRect.pivot            = Vector2.one;
        tRect.sizeDelta        = new Vector2(120f, 32f);
        tRect.anchoredPosition = new Vector2(-8f, -8f);
        toggleBtnGO.AddComponent<Image>().color = ColBtnNormal;
        var toggleBtn = toggleBtnGO.AddComponent<Button>();
        ApplyButtonColors(toggleBtn);
        MakeText(toggleBtnGO, "⚙ Dev Console", 12, ColText, TextAnchor.MiddleCenter);
        toggleBtn.onClick.AddListener(() => SetPanelVisible(!_panelVisible));

        // ── Panel (left-center, fixed size)
        var panelGO   = MakeRect("Panel", canvasGO.transform);
        var panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin        = new Vector2(0f, 0.5f);
        panelRect.anchorMax        = new Vector2(0f, 0.5f);
        panelRect.pivot            = new Vector2(0f, 0.5f);
        panelRect.sizeDelta        = new Vector2(360f, 520f);
        panelRect.anchoredPosition = new Vector2(8f, 0f);
        panelGO.AddComponent<Image>().color = ColPanel;
        _panelGroup = panelGO.AddComponent<CanvasGroup>();

        // ── Header row (inside panel)
        var headerGO   = MakeRect("Header", panelGO.transform);
        var headerRect = headerGO.GetComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0f, 1f);
        headerRect.anchorMax = Vector2.one;
        headerRect.pivot     = new Vector2(0.5f, 1f);
        headerRect.sizeDelta = new Vector2(0f, 36f);
        headerRect.anchoredPosition = Vector2.zero;
        headerGO.AddComponent<Image>().color = ColPanelHead;
        var headerTxt = MakeText(headerGO, "⚙ Dev Console", 13, ColHeader, TextAnchor.MiddleLeft);
        {
            var rt = headerTxt.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(0.82f, 1f);
            rt.offsetMin = new Vector2(10f, 0f);
            rt.offsetMax = Vector2.zero;
        }

        // Close [×] button in header
        var closeBtnGO   = MakeRect("CloseBtn", headerGO.transform);
        var closeBtnRect = closeBtnGO.GetComponent<RectTransform>();
        closeBtnRect.anchorMin        = new Vector2(1f, 0f);
        closeBtnRect.anchorMax        = Vector2.one;
        closeBtnRect.pivot            = Vector2.one;
        closeBtnRect.sizeDelta        = new Vector2(36f, 0f);
        closeBtnRect.anchoredPosition = Vector2.zero;
        closeBtnGO.AddComponent<Image>().color = ColBtnRed;
        var closeBtn = closeBtnGO.AddComponent<Button>();
        ApplyButtonColors(closeBtn);
        MakeText(closeBtnGO, "×", 16, ColText, TextAnchor.MiddleCenter);
        closeBtn.onClick.AddListener(() => SetPanelVisible(false));

        // ── Scroll view (fills panel below header)
        var scrollGO   = MakeRect("ScrollView", panelGO.transform);
        var scrollRect = scrollGO.GetComponent<RectTransform>();
        scrollRect.anchorMin = Vector2.zero;
        scrollRect.anchorMax = Vector2.one;
        scrollRect.offsetMin = Vector2.zero;
        scrollRect.offsetMax = new Vector2(0f, -36f);

        var sr = scrollGO.AddComponent<ScrollRect>();
        sr.horizontal        = false;
        sr.scrollSensitivity = 20f;

        // Viewport
        var vpGO   = MakeRect("Viewport", scrollGO.transform);
        var vpRect = vpGO.GetComponent<RectTransform>();
        vpRect.anchorMin = Vector2.zero;
        vpRect.anchorMax = Vector2.one;
        vpRect.sizeDelta = Vector2.zero;
        vpRect.pivot     = new Vector2(0.5f, 1f);
        vpGO.AddComponent<Image>().color = Color.white; // alpha=1 needed for mask to work
        vpGO.AddComponent<Mask>().showMaskGraphic = false; // don't render the white image
        sr.viewport = vpRect;

        // Content
        var contentGO   = MakeRect("Content", vpGO.transform);
        var contentRect = contentGO.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot     = new Vector2(0.5f, 1f);
        contentRect.sizeDelta = Vector2.zero;
        sr.content = contentRect;

        var vlg = contentGO.AddComponent<VerticalLayoutGroup>();
        vlg.padding            = new RectOffset(8, 8, 6, 8);
        vlg.spacing            = 4f;
        vlg.childControlHeight = true;
        vlg.childControlWidth  = true;
        vlg.childForceExpandHeight = false;
        vlg.childForceExpandWidth  = true;

        var csf = contentGO.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // ─── PLAYER ───────────────────────────────────────────────────────
        AddSectionHeader(contentGO, "── PLAYER ──");
        (_sliderWalk, _lblWalkVal) = AddSliderRow(contentGO, "Walk Speed", 1f, 20f, DEF_WALK_SPEED, "F1");

        // ─── CAMERA ───────────────────────────────────────────────────────
        AddSectionHeader(contentGO, "── CAMERA ──");
        (_sliderZoom, _lblZoomVal) = AddSliderRow(contentGO, "Zoom Distance", 4f, 35f, DEF_CAMERA_ZOOM, "F1");

        // ─── DAY / NIGHT ───────────────────────────────────────────────────
        AddSectionHeader(contentGO, "── DAY / NIGHT ──");
        _toggleDayNight = AddToggleRow(contentGO, "Enable Day/Night", DEF_DAY_NIGHT);
        (_sliderDayDuration, _lblDayDurationVal) = AddSliderRow(contentGO, "Cycle Duration", 10f, 600f, DEF_DAY_DURATION, "F0");
        _lblDayDurationVal.text = DEF_DAY_DURATION.ToString("F0") + "s";
        (_sliderTimeOfDay, _lblTimeOfDayVal) = AddSliderRow(contentGO, "Time of Day", 0f, 1f, DEF_TIME_OF_DAY, "F2");

        // ─── WORLD ───────────────────────────────────────────────────────
        AddSectionHeader(contentGO, "── WORLD ──");
        (_sliderCurvature, _lblCurvatureVal) = AddSliderRow(contentGO, "Curvature", 0f, 0.008f, DEF_CURVATURE, "F4");

        // ─── KEY BINDINGS ─────────────────────────────────────────────────
        AddSectionHeader(contentGO, "── KEY BINDINGS ──");
        (_btnConsoleKey, _txtConsoleKey) = AddRebindRow(contentGO, "Console Key", "Console");
        (_btnSprintKey,  _txtSprintKey)  = AddRebindRow(contentGO, "Sprint",      "Sprint");
        (_btnInteractKey, _txtInteractKey) = AddRebindRow(contentGO, "Interact",  "Interact");
        (_btnJumpKey,    _txtJumpKey)    = AddRebindRow(contentGO, "Jump",        "Jump");
        AddNoteRow(contentGO, "Movement: WASD / Arrow keys (engine fixed)");

        // ─── RESET ────────────────────────────────────────────────────────
        AddSectionHeader(contentGO, "────────────────");
        var resetBtn = AddFullButton(contentGO, "↺  Reset Defaults", new Color(0.25f, 0.15f, 0.15f, 1f));
        resetBtn.onClick.AddListener(ResetDefaults);

        // Force layout rebuild so ContentSizeFitter calculates immediately in Awake
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);

        // Start hidden; toggle with backtick or the corner button
        SetPanelVisible(false);
    }

    // =========================================================================
    //  UI FACTORY HELPERS
    // =========================================================================

    static GameObject MakeRect(string name, Transform parent)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        return go;
    }

    GameObject MakeText(GameObject parent, string text, int size, Color color, TextAnchor align)
    {
        var go = MakeRect("Text", parent.transform);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        var t = go.AddComponent<Text>();
        t.text      = text;
        t.font      = _font;
        t.fontSize  = size;
        t.color     = color;
        t.alignment = align;
        return go;
    }

    void AddSectionHeader(GameObject content, string label)
    {
        var go = MakeRect("SectionHeader", content.transform);
        go.AddComponent<LayoutElement>().preferredHeight = 20f;
        var t = go.AddComponent<Text>();
        t.text      = label;
        t.font      = _font;
        t.fontSize  = 10;
        t.color     = ColHeader;
        t.alignment = TextAnchor.MiddleLeft;
    }

    (Slider, Text) AddSliderRow(GameObject content, string label, float min, float max, float def, string fmt)
    {
        var rowGO = MakeRect("Row_" + label, content.transform);
        var le    = rowGO.AddComponent<LayoutElement>();
        le.preferredHeight = 26f;

        var hlg = rowGO.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing            = 6f;
        hlg.childControlHeight = true;
        hlg.childControlWidth  = true;
        hlg.childForceExpandHeight = true;
        hlg.childForceExpandWidth  = false;

        // Label
        var lblGO = MakeRect("Lbl", rowGO.transform);
        lblGO.AddComponent<LayoutElement>().preferredWidth = 115f;
        var lbl = lblGO.AddComponent<Text>();
        lbl.text      = label;
        lbl.font      = _font;
        lbl.fontSize  = 11;
        lbl.color     = ColTextDim;
        lbl.alignment = TextAnchor.MiddleLeft;

        // Slider container
        var sliderGO = MakeRect("Slider", rowGO.transform);
        sliderGO.AddComponent<LayoutElement>().flexibleWidth = 1f;

        // Background track
        var bgGO = MakeRect("BG", sliderGO.transform);
        var bgRT = bgGO.GetComponent<RectTransform>();
        bgRT.anchorMin = new Vector2(0f, 0.25f);
        bgRT.anchorMax = new Vector2(1f, 0.75f);
        bgRT.sizeDelta = Vector2.zero;
        bgGO.AddComponent<Image>().color = ColSliderBg;

        // Fill Area
        var faGO = MakeRect("FillArea", sliderGO.transform);
        var faRT = faGO.GetComponent<RectTransform>();
        faRT.anchorMin = new Vector2(0f, 0.25f);
        faRT.anchorMax = new Vector2(1f, 0.75f);
        faRT.offsetMin = new Vector2(0f, 0f);
        faRT.offsetMax = new Vector2(-10f, 0f);

        var fillGO = MakeRect("Fill", faGO.transform);
        var fillRT = fillGO.GetComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = new Vector2(0f, 1f);
        fillRT.sizeDelta = Vector2.zero;
        fillGO.AddComponent<Image>().color = ColSliderFill;

        // Handle slide area
        var haGO = MakeRect("HandleArea", sliderGO.transform);
        var haRT = haGO.GetComponent<RectTransform>();
        haRT.anchorMin = Vector2.zero;
        haRT.anchorMax = Vector2.one;
        haRT.offsetMin = Vector2.zero;
        haRT.offsetMax = Vector2.zero;

        var handleGO = MakeRect("Handle", haGO.transform);
        var handleRT = handleGO.GetComponent<RectTransform>();
        handleRT.anchorMin = new Vector2(0f, 0f);
        handleRT.anchorMax = new Vector2(0f, 1f);
        handleRT.sizeDelta = new Vector2(12f, 0f);
        var handleImg = handleGO.AddComponent<Image>();
        handleImg.color = Color.white;

        // Assemble slider
        var slider = sliderGO.AddComponent<Slider>();
        slider.targetGraphic = handleImg;
        slider.fillRect      = fillRT;
        slider.handleRect    = handleRT;
        slider.minValue      = min;
        slider.maxValue      = max;
        slider.value         = def;

        // Value label
        var valGO = MakeRect("Val", rowGO.transform);
        valGO.AddComponent<LayoutElement>().preferredWidth = 52f;
        var valTxt = valGO.AddComponent<Text>();
        valTxt.text      = def.ToString(fmt);
        valTxt.font      = _font;
        valTxt.fontSize  = 11;
        valTxt.color     = ColText;
        valTxt.alignment = TextAnchor.MiddleRight;

        return (slider, valTxt);
    }

    Toggle AddToggleRow(GameObject content, string label, bool defaultVal)
    {
        var rowGO = MakeRect("ToggleRow_" + label, content.transform);
        rowGO.AddComponent<LayoutElement>().preferredHeight = 26f;
        var hlg = rowGO.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing            = 8f;
        hlg.childControlHeight = true;
        hlg.childControlWidth  = true;
        hlg.childForceExpandHeight = true;
        hlg.childForceExpandWidth  = false;

        // Checkbox box
        var boxGO = MakeRect("Box", rowGO.transform);
        boxGO.AddComponent<LayoutElement>().preferredWidth = 20f;
        var boxImg = boxGO.AddComponent<Image>();
        boxImg.color = ColBtnNormal;

        // Checkmark
        var checkGO = MakeRect("Checkmark", boxGO.transform);
        var checkRT = checkGO.GetComponent<RectTransform>();
        checkRT.anchorMin = new Vector2(0.1f, 0.1f);
        checkRT.anchorMax = new Vector2(0.9f, 0.9f);
        checkRT.sizeDelta = Vector2.zero;
        var checkImg = checkGO.AddComponent<Image>();
        checkImg.color = ColSliderFill;

        // Toggle component
        var toggle = boxGO.AddComponent<Toggle>();
        toggle.targetGraphic = boxImg;
        toggle.graphic       = checkImg;
        toggle.isOn          = defaultVal;

        // Label
        var lblGO = MakeRect("Lbl", rowGO.transform);
        lblGO.AddComponent<LayoutElement>().preferredWidth = 200f;
        var lbl = lblGO.AddComponent<Text>();
        lbl.text      = label;
        lbl.font      = _font;
        lbl.fontSize  = 11;
        lbl.color     = ColTextDim;
        lbl.alignment = TextAnchor.MiddleLeft;

        return toggle;
    }

    (Button, Text) AddRebindRow(GameObject content, string label, string target)
    {
        var rowGO = MakeRect("RebindRow_" + label, content.transform);
        rowGO.AddComponent<LayoutElement>().preferredHeight = 26f;
        var hlg = rowGO.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing            = 6f;
        hlg.childControlHeight = true;
        hlg.childControlWidth  = true;
        hlg.childForceExpandHeight = true;
        hlg.childForceExpandWidth  = false;

        // Label
        var lblGO = MakeRect("Lbl", rowGO.transform);
        lblGO.AddComponent<LayoutElement>().preferredWidth = 115f;
        var lbl = lblGO.AddComponent<Text>();
        lbl.text      = label;
        lbl.font      = _font;
        lbl.fontSize  = 11;
        lbl.color     = ColTextDim;
        lbl.alignment = TextAnchor.MiddleLeft;

        // Rebind button
        var btnGO = MakeRect("RebindBtn", rowGO.transform);
        btnGO.AddComponent<LayoutElement>().flexibleWidth = 1f;
        btnGO.AddComponent<Image>().color = ColBtnNormal;
        var btn = btnGO.AddComponent<Button>();
        ApplyButtonColors(btn);

        var keyTxt = MakeRect("KeyText", btnGO.transform);
        var keyTxtRT = keyTxt.GetComponent<RectTransform>();
        keyTxtRT.anchorMin = Vector2.zero;
        keyTxtRT.anchorMax = Vector2.one;
        keyTxtRT.sizeDelta = Vector2.zero;
        var keyT = keyTxt.AddComponent<Text>();
        keyT.text      = "...";
        keyT.font      = _font;
        keyT.fontSize  = 11;
        keyT.color     = ColText;
        keyT.alignment = TextAnchor.MiddleCenter;

        btn.onClick.AddListener(() => StartRebind(target));
        return (btn, keyT);
    }

    void AddNoteRow(GameObject content, string note)
    {
        var go = MakeRect("Note", content.transform);
        go.AddComponent<LayoutElement>().preferredHeight = 20f;
        var t = go.AddComponent<Text>();
        t.text      = note;
        t.font      = _font;
        t.fontSize  = 10;
        t.color     = new Color(0.5f, 0.5f, 0.5f, 1f);
        t.alignment = TextAnchor.MiddleLeft;
        t.fontStyle = FontStyle.Italic;
    }

    Button AddFullButton(GameObject content, string label, Color bgColor)
    {
        var go = MakeRect("Btn_" + label, content.transform);
        go.AddComponent<LayoutElement>().preferredHeight = 30f;
        go.AddComponent<Image>().color = bgColor;
        var btn = go.AddComponent<Button>();
        ApplyButtonColors(btn);
        // Text must be a child — can't co-exist with Image on the same GameObject
        MakeText(go, label, 12, ColText, TextAnchor.MiddleCenter);
        return btn;
    }

    static void ApplyButtonColors(Button btn)
    {
        var cb = btn.colors;
        cb.normalColor      = Color.white;
        cb.highlightedColor = new Color(1.2f, 1.2f, 1.2f, 1f);
        cb.pressedColor     = new Color(0.7f, 0.7f, 0.7f, 1f);
        cb.selectedColor    = Color.white;
        cb.colorMultiplier  = 1f;
        btn.colors          = cb;
    }
}
