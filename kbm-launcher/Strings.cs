// UI text for the launcher. Order of every entry: en, tr, es, pt, fr, de, ru, zh.
using System;
using System.Collections.Generic;

namespace ChiakiMouseLauncher
{
	static class Strings
	{
		public static readonly string[] Codes = { "en", "tr", "es", "pt", "fr", "de", "ru", "zh" };
		public static readonly string[] Names = { "English", "Türkçe", "Español", "Português", "Français", "Deutsch", "Русский", "中文 (简体)" };

		static int lang;
		public static string Code { get { return Codes[lang]; } }

		public static void SetLanguage(string code)
		{
			lang = Math.Max(0, Array.IndexOf(Codes, code));
		}

		public static string T(string key)
		{
			string[] v;
			if (!table.TryGetValue(key, out v)) return key;
			return v[lang] ?? v[0];
		}

		public static string F(string key, params object[] args)
		{
			return string.Format(T(key), args);
		}

		static readonly Dictionary<string, string[]> table = new Dictionary<string, string[]>
		{
			{ "title", new[] { "Keyboard + Mouse for chiaki-ng", "chiaki-ng için Klavye + Mouse", "Teclado + ratón para chiaki-ng", "Teclado + mouse para chiaki-ng", "Clavier + souris pour chiaki-ng", "Tastatur + Maus für chiaki-ng", "Клавиатура и мышь для chiaki-ng", "chiaki-ng 键鼠映射" } },
			{ "subtitle", new[] { "Click an action, then press the key you want. Right-click: remove the key.", "Bir işleme tıklayın, sonra atamak istediğiniz tuşa basın. Sağ tık: tuşu kaldır.", "Haz clic en una acción y pulsa la tecla que quieras. Clic derecho: quitar la tecla.", "Clique em uma ação e pressione a tecla desejada. Botão direito: remover a tecla.", "Cliquez sur une action, puis appuyez sur la touche voulue. Clic droit : retirer la touche.", "Aktion anklicken, dann die gewünschte Taste drücken. Rechtsklick: Taste entfernen.", "Нажмите на действие, затем на нужную клавишу. Правый клик — убрать клавишу.", "点击一个操作，然后按下要绑定的键。右键：移除按键。" } },
			{ "language", new[] { "Language", "Dil", "Idioma", "Idioma", "Langue", "Sprache", "Язык", "语言" } },

			{ "sec_move", new[] { "Movement", "Hareket", "Movimiento", "Movimento", "Déplacement", "Bewegung", "Движение", "移动" } },
			{ "sec_combat", new[] { "Combat", "Savaş", "Combate", "Combate", "Combat", "Kampf", "Бой", "战斗" } },
			{ "sec_interact", new[] { "Interaction & menus", "Etkileşim ve menüler", "Interacción y menús", "Interação e menus", "Interaction et menus", "Interaktion & Menüs", "Взаимодействие и меню", "交互与菜单" } },
			{ "sec_vehicle", new[] { "Vehicle", "Araç", "Vehículo", "Veículo", "Véhicule", "Fahrzeug", "Транспорт", "载具" } },
			{ "sec_other", new[] { "Other", "Diğer", "Otros", "Outros", "Autres", "Sonstiges", "Прочее", "其他" } },

			{ "a_forward", new[] { "Forward", "İleri", "Adelante", "Frente", "Avancer", "Vorwärts", "Вперёд", "前进" } },
			{ "h_forward", new[] { "Vehicle mode: accelerate", "Araç modunda: gaz", "Modo vehículo: acelerar", "Modo veículo: acelerar", "Mode véhicule : accélérer", "Fahrzeugmodus: Gas", "Режим транспорта: газ", "载具模式：油门" } },
			{ "a_back", new[] { "Back", "Geri", "Atrás", "Trás", "Reculer", "Rückwärts", "Назад", "后退" } },
			{ "h_back", new[] { "Vehicle mode: brake / reverse", "Araç modunda: fren / geri vites", "Modo vehículo: frenar / marcha atrás", "Modo veículo: freio / ré", "Mode véhicule : freiner / marche arrière", "Fahrzeugmodus: Bremse / Rückwärts", "Режим транспорта: тормоз / задний ход", "载具模式：刹车/倒车" } },
			{ "a_left", new[] { "Left", "Sola", "Izquierda", "Esquerda", "Gauche", "Links", "Влево", "向左" } },
			{ "a_right", new[] { "Right", "Sağa", "Derecha", "Direita", "Droite", "Rechts", "Вправо", "向右" } },
			{ "a_run", new[] { "Run", "Koş", "Correr", "Correr", "Courir", "Rennen", "Бег", "奔跑" } },
			{ "a_jump", new[] { "Jump / Climb", "Zıpla / Tırman", "Saltar / Trepar", "Pular / Escalar", "Sauter / Grimper", "Springen / Klettern", "Прыжок / подъём", "跳跃/攀爬" } },
			{ "a_crouch", new[] { "Crouch", "Çömel", "Agacharse", "Agachar", "S'accroupir", "Ducken", "Присесть", "蹲下" } },
			{ "a_cover", new[] { "Cover", "Siper al", "Cobertura", "Cobertura", "Se mettre à couvert", "Deckung", "Укрытие", "掩护" } },

			{ "a_aim", new[] { "Aim / Interact", "Nişan al / Etkileşim", "Apuntar / Interactuar", "Mirar / Interagir", "Viser / Interagir", "Zielen / Interagieren", "Прицел / взаимодействие", "瞄准/互动" } },
			{ "h_aim", new[] { "Right mouse button too", "Mouse sağ tık da yapar", "También clic derecho", "Também botão direito", "Aussi clic droit", "Auch rechte Maustaste", "Также правая кнопка мыши", "鼠标右键同样可用" } },
			{ "a_shoot", new[] { "Shoot", "Ateş et", "Disparar", "Atirar", "Tirer", "Schießen", "Стрельба", "射击" } },
			{ "h_shoot", new[] { "Left mouse button too", "Mouse sol tık da yapar", "También clic izquierdo", "Também botão esquerdo", "Aussi clic gauche", "Auch linke Maustaste", "Также левая кнопка мыши", "鼠标左键同样可用" } },
			{ "a_melee", new[] { "Melee", "Yakın dövüş", "Cuerpo a cuerpo", "Corpo a corpo", "Corps à corps", "Nahkampf", "Ближний бой", "近战" } },
			{ "a_holster", new[] { "Holster", "Silahı kılıfına koy", "Enfundar", "Guardar arma", "Ranger l'arme", "Waffe wegstecken", "Убрать оружие", "收起武器" } },
			{ "a_lookbehind", new[] { "Look behind", "Arkana bak", "Mirar atrás", "Olhar para trás", "Regarder derrière", "Nach hinten schauen", "Оглянуться", "向后看" } },
			{ "h_middle", new[] { "Middle mouse button too", "Mouse orta tık da yapar", "También botón central", "Também botão do meio", "Aussi clic molette", "Auch mittlere Maustaste", "Также средняя кнопка мыши", "鼠标中键同样可用" } },

			{ "a_interact", new[] { "Interact / Enter vehicle", "Etkileşim / Araca bin", "Interactuar / Subir al vehículo", "Interagir / Entrar no veículo", "Interagir / Monter dans un véhicule", "Interagieren / Einsteigen", "Взаимодействие / сесть в транспорт", "互动/上车" } },
			{ "a_map", new[] { "Map", "Harita", "Mapa", "Mapa", "Carte", "Karte", "Карта", "地图" } },
			{ "a_pause", new[] { "Pause", "Duraklat", "Pausa", "Pausar", "Pause", "Pause", "Пауза", "暂停" } },
			{ "a_phone", new[] { "Phone", "Telefon", "Teléfono", "Celular", "Téléphone", "Handy", "Телефон", "手机" } },
			{ "a_dpad_down", new[] { "D-pad down", "D-pad aşağı", "Cruceta abajo", "Direcional baixo", "Croix bas", "Steuerkreuz unten", "Крестовина вниз", "方向键下" } },
			{ "a_dpad_left", new[] { "D-pad left", "D-pad sol", "Cruceta izquierda", "Direcional esquerda", "Croix gauche", "Steuerkreuz links", "Крестовина влево", "方向键左" } },
			{ "a_dpad_right", new[] { "D-pad right", "D-pad sağ", "Cruceta derecha", "Direcional direita", "Croix droite", "Steuerkreuz rechts", "Крестовина вправо", "方向键右" } },
			{ "h_dpad", new[] { "Radio / objective / explosives", "Radyo / hedef / patlayıcı", "Radio / objetivo / explosivos", "Rádio / objetivo / explosivos", "Radio / objectif / explosifs", "Radio / Ziel / Sprengstoff", "Радио / цель / взрывчатка", "电台/目标/炸药" } },

			{ "a_drive", new[] { "Vehicle mode on / off", "Araç modu aç / kapat", "Modo vehículo sí / no", "Modo veículo liga / desliga", "Mode véhicule on / off", "Fahrzeugmodus an / aus", "Режим транспорта вкл / выкл", "载具模式开/关" } },
			{ "h_drive", new[] {
				"Press it when you get in: W / S = gas / brake, left click shoots (R1). It turns off by itself when you get out (△).",
				"Araca binince basın: W / S = gaz / fren, sol tık ateş (R1). Araçtan inince (△) kendiliğinden kapanır.",
				"Púlsalo al subir: W / S = acelerar / frenar, clic izquierdo dispara (R1). Se desactiva solo al bajar (△).",
				"Aperte ao entrar: W / S = acelerar / frear, botão esquerdo atira (R1). Desliga sozinho ao sair (△).",
				"Appuyez en montant : W / S = accélérer / freiner, clic gauche tire (R1). Se désactive seul en descendant (△).",
				"Beim Einsteigen drücken: W / S = Gas / Bremse, Linksklick schießt (R1). Schaltet sich beim Aussteigen (△) selbst aus.",
				"Нажмите, сев в машину: W / S — газ / тормоз, ЛКМ — стрельба (R1). Выключается сам, когда вы выходите (△).",
				"上车时按下：W / S = 油门 / 刹车，左键射击 (R1)。下车（△）时自动关闭。" } },
			{ "veh_exit_e", new[] { "Turn vehicle mode off when you press △ (exit vehicle)", "△'ye basınca (araçtan in) araç modunu kapat", "Desactivar el modo vehículo al pulsar △ (bajar)", "Desligar o modo veículo ao apertar △ (sair)", "Désactiver le mode véhicule avec △ (descendre)", "Fahrzeugmodus mit △ (aussteigen) ausschalten", "Выключать режим транспорта по △ (выйти)", "按 △（下车）时关闭载具模式" } },
			{ "veh_exit_aim", new[] { "Turn it off on right click (aiming on foot)", "Sağ tıkta kapat (yayayken nişan)", "Desactivarlo con clic derecho (apuntar a pie)", "Desligar com botão direito (mirar a pé)", "Le désactiver au clic droit (viser à pied)", "Mit Rechtsklick ausschalten (zu Fuß zielen)", "Выключать по ПКМ (прицел пешком)", "右键时关闭（步行瞄准）" } },
			{ "sec_menus", new[] { "Menus", "Menüler", "Menús", "Menus", "Menus", "Menüs", "Меню", "菜单" } },
			{ "a_confirm", new[] { "Confirm", "Onayla", "Confirmar", "Confirmar", "Valider", "Bestätigen", "Подтвердить", "确认" } },
			{ "h_confirm", new[] { "Menus, phone… (second key for ✕)", "Menüler, telefon… (✕ için ikinci tuş)", "Menús, teléfono… (segunda tecla de ✕)", "Menus, celular… (segunda tecla do ✕)", "Menus, téléphone… (2e touche pour ✕)", "Menüs, Handy… (zweite Taste für ✕)", "Меню, телефон… (вторая клавиша ✕)", "菜单、手机…（✕ 的第二个键）" } },
			{ "a_cancel", new[] { "Back / Cancel", "Geri / İptal", "Atrás / Cancelar", "Voltar / Cancelar", "Retour / Annuler", "Zurück / Abbrechen", "Назад / отмена", "返回/取消" } },
			{ "h_cancel", new[] { "Menus, phone… (second key for ○)", "Menüler, telefon… (○ için ikinci tuş)", "Menús, teléfono… (segunda tecla de ○)", "Menus, celular… (segunda tecla do ○)", "Menus, téléphone… (2e touche pour ○)", "Menüs, Handy… (zweite Taste für ○)", "Меню, телефон… (вторая клавиша ○)", "菜单、手机…（○ 的第二个键）" } },
			{ "a_focus", new[] { "Focus mode", "Odak modu", "Modo concentración", "Modo foco", "Mode focus", "Fokusmodus", "Режим фокуса", "专注模式" } },
			{ "h_focus", new[] { "L3 + R3 together", "L3 + R3 birlikte", "L3 + R3 a la vez", "L3 + R3 juntos", "L3 + R3 ensemble", "L3 + R3 zusammen", "L3 + R3 вместе", "同时按 L3 + R3" } },
			{ "veh_info", new[] {
				"In GTA VI, vehicle mode can switch itself on and off from the PS5's trigger feedback. In other games, or if it misses, use its key (V):",
				"GTA VI'da araç modu, PS5'in tetik titreşimlerinden araca bindiğinizi anlayıp kendiliğinden açılıp kapanabilir. Diğer oyunlarda veya kaçırırsa kendi tuşunu (V) kullanın:",
				"En GTA VI, el modo vehículo puede activarse y desactivarse solo según la respuesta de los gatillos de la PS5. En otros juegos, o si falla, usa su tecla (V):",
				"No GTA VI, o modo veículo pode ligar e desligar sozinho pela resposta dos gatilhos do PS5. Em outros jogos, ou se falhar, use a tecla (V):",
				"Dans GTA VI, le mode véhicule peut s'activer et se désactiver seul grâce au retour des gâchettes de la PS5. Dans d'autres jeux, ou s'il se trompe, utilisez sa touche (V) :",
				"In GTA VI kann sich der Fahrzeugmodus anhand des Trigger-Feedbacks der PS5 selbst ein- und ausschalten. In anderen Spielen oder wenn er es verpasst: seine Taste (V):",
				"В GTA VI режим транспорта может включаться и выключаться сам по отдаче курков PS5. В других играх или при ошибке используйте его клавишу (V):",
				"在 GTA VI 中，载具模式可根据 PS5 的扳机反馈自动开关。其他游戏或识别失误时，请用载具模式键（V）：" } },
			{ "veh_detect", new[] { "Detect vehicles from trigger feedback (GTA VI)", "Aracı tetik titreşiminden algıla (GTA VI)", "Detectar vehículos por los gatillos (GTA VI)", "Detectar veículos pelos gatilhos (GTA VI)", "Détecter les véhicules via les gâchettes (GTA VI)", "Fahrzeuge am Trigger-Feedback erkennen (GTA VI)", "Определять транспорт по отдаче курков (GTA VI)", "通过扳机反馈识别载具（GTA VI）" } },
			{ "veh_auto", new[] { "Switch automatically with △", "△ ile otomatik geçiş", "Cambiar automáticamente con △", "Trocar automaticamente com △", "Basculer automatiquement avec △", "Automatisch mit △ wechseln", "Переключать автоматически по △", "按 △ 时自动切换" } },
			{ "veh_beep", new[] { "Sound when vehicle mode changes", "Araç modu değişince ses çıkar", "Sonido al cambiar el modo vehículo", "Som ao mudar o modo veículo", "Son au changement de mode", "Ton beim Moduswechsel", "Звук при смене режима", "切换载具模式时提示音" } },
			{ "veh_right", new[] { "Right click in vehicle (aim)", "Araçta sağ tık (nişan)", "Clic derecho en vehículo (apuntar)", "Botão direito no veículo (mirar)", "Clic droit en véhicule (viser)", "Rechtsklick im Fahrzeug (zielen)", "ПКМ в машине (прицел)", "车内右键（瞄准）" } },
			{ "veh_left", new[] { "Left click in vehicle (shoot)", "Araçta sol tık (ateş)", "Clic izquierdo en vehículo (disparar)", "Botão esquerdo no veículo (atirar)", "Clic gauche en véhicule (tirer)", "Linksklick im Fahrzeug (schießen)", "ЛКМ в машине (стрельба)", "车内左键（射击）" } },

			{ "a_ps", new[] { "PS button", "PS tuşu", "Botón PS", "Botão PS", "Touche PS", "PS-Taste", "Кнопка PS", "PS 键" } },
			{ "a_create", new[] { "Create", "Create", "Create", "Create", "Create", "Create", "Create", "Create" } },
			{ "a_cam_up", new[] { "Camera up", "Kamera yukarı", "Cámara arriba", "Câmera para cima", "Caméra haut", "Kamera hoch", "Камера вверх", "视角上" } },
			{ "a_cam_down", new[] { "Camera down", "Kamera aşağı", "Cámara abajo", "Câmera para baixo", "Caméra bas", "Kamera runter", "Камера вниз", "视角下" } },
			{ "a_cam_left", new[] { "Camera left", "Kamera sola", "Cámara izquierda", "Câmera esquerda", "Caméra gauche", "Kamera links", "Камера влево", "视角左" } },
			{ "a_cam_right", new[] { "Camera right", "Kamera sağa", "Cámara derecha", "Câmera direita", "Caméra droite", "Kamera rechts", "Камера вправо", "视角右" } },
			{ "h_cam", new[] { "Keyboard instead of mouse", "Mouse yerine klavyeyle", "Con teclado en vez de ratón", "Pelo teclado em vez do mouse", "Au clavier au lieu de la souris", "Tastatur statt Maus", "Клавиатурой вместо мыши", "用键盘代替鼠标" } },

			{ "press_key", new[] { "Press a key…", "Bir tuşa basın…", "Pulsa una tecla…", "Pressione uma tecla…", "Appuyez sur une touche…", "Taste drücken…", "Нажмите клавишу…", "请按键…" } },
			{ "mouse", new[] { "MOUSE", "MOUSE", "RATÓN", "MOUSE", "SOURIS", "MAUS", "МЫШЬ", "鼠标" } },
			{ "mouse_info", new[] {
				"Move = camera · Left click = shoot (R2) · Right click = aim (L2)\nMiddle click = R3 · Side buttons = L1 / R1 · F8 = mouse mode on/off",
				"Hareket = kamera · Sol tık = ateş (R2) · Sağ tık = nişan (L2)\nOrta tık = R3 · Yan tuşlar = L1 / R1 · F8 = mouse modu aç/kapat",
				"Mover = cámara · Clic izq. = disparar (R2) · Clic der. = apuntar (L2)\nClic central = R3 · Botones laterales = L1 / R1 · F8 = modo ratón sí/no",
				"Mover = câmera · Botão esq. = atirar (R2) · Botão dir. = mirar (L2)\nBotão do meio = R3 · Botões laterais = L1 / R1 · F8 = modo mouse liga/desliga",
				"Déplacer = caméra · Clic gauche = tirer (R2) · Clic droit = viser (L2)\nClic molette = R3 · Boutons latéraux = L1 / R1 · F8 = mode souris on/off",
				"Bewegen = Kamera · Linksklick = schießen (R2) · Rechtsklick = zielen (L2)\nMittelklick = R3 · Seitentasten = L1 / R1 · F8 = Mausmodus an/aus",
				"Движение = камера · ЛКМ = стрельба (R2) · ПКМ = прицел (L2)\nСКМ = R3 · Боковые = L1 / R1 · F8 = режим мыши вкл/выкл",
				"移动 = 视角 · 左键 = 射击 (R2) · 右键 = 瞄准 (L2)\n中键 = R3 · 侧键 = L1 / R1 · F8 = 鼠标模式开/关" } },
			{ "sensitivity", new[] { "Sensitivity", "Hassasiyet", "Sensibilidad", "Sensibilidade", "Sensibilité", "Empfindlichkeit", "Чувствительность", "灵敏度" } },
			{ "deadzone", new[] { "Deadzone offset", "Ölü bölge telafisi", "Zona muerta", "Zona morta", "Zone morte", "Deadzone-Ausgleich", "Мёртвая зона", "死区补偿" } },
			{ "invert_y", new[] { "Invert Y axis", "Y eksenini ters çevir", "Invertir eje Y", "Inverter eixo Y", "Inverser l'axe Y", "Y-Achse umkehren", "Инвертировать ось Y", "反转 Y 轴" } },
			{ "mouse_on", new[] { "Mouse mode on when connecting", "Bağlanınca mouse modu açık olsun", "Modo ratón activo al conectar", "Modo mouse ligado ao conectar", "Mode souris actif à la connexion", "Mausmodus beim Verbinden an", "Режим мыши при подключении", "连接时开启鼠标模式" } },
			{ "stream", new[] { "STREAM", "YAYIN", "TRANSMISIÓN", "TRANSMISSÃO", "FLUX", "STREAM", "ТРАНСЛЯЦИЯ", "串流" } },
			{ "fps_info", new[] { "60 → 120/60 with Lossless Scaling X2", "60 → Lossless Scaling X2 ile 120/60", "60 → 120/60 con Lossless Scaling X2", "60 → 120/60 com Lossless Scaling X2", "60 → 120/60 avec Lossless Scaling X2", "60 → 120/60 mit Lossless Scaling X2", "60 → 120/60 с Lossless Scaling X2", "60 → 配合 Lossless Scaling X2 为 120/60" } },

			{ "start", new[] { "START", "BAŞLAT", "INICIAR", "INICIAR", "LANCER", "STARTEN", "ЗАПУСК", "启动" } },
			{ "save", new[] { "Save", "Kaydet", "Guardar", "Salvar", "Enregistrer", "Speichern", "Сохранить", "保存" } },
			{ "defaults", new[] { "Defaults", "Varsayılanlar", "Predeterminado", "Padrão", "Par défaut", "Standard", "По умолчанию", "默认" } },
			{ "saved", new[] { "Saved.", "Kaydedildi.", "Guardado.", "Salvo.", "Enregistré.", "Gespeichert.", "Сохранено.", "已保存。" } },
			{ "cleared", new[] { "Key removed from \"{0}\".", "\"{0}\" tuşu kaldırıldı.", "Tecla quitada de \"{0}\".", "Tecla removida de \"{0}\".", "Touche retirée de « {0} ».", "Taste von „{0}“ entfernt.", "Клавиша снята с «{0}».", "已移除“{0}”的按键。" } },
			{ "capture_prompt", new[] { "Press a key for \"{0}\". Click the row again to cancel.", "\"{0}\" için bir tuşa basın. Vazgeçmek için satıra tekrar tıklayın.", "Pulsa una tecla para \"{0}\". Vuelve a hacer clic para cancelar.", "Pressione uma tecla para \"{0}\". Clique de novo para cancelar.", "Appuyez sur une touche pour « {0} ». Recliquez pour annuler.", "Taste für „{0}“ drücken. Erneut klicken zum Abbrechen.", "Нажмите клавишу для «{0}». Щёлкните ещё раз для отмены.", "请为“{0}”按键。再次点击取消。" } },
			{ "reserved", new[] { "F8 (mouse mode) and F11 (full screen) are reserved by chiaki-ng.", "F8 (mouse modu) ve F11 (tam ekran) chiaki-ng'ye ayrılmış.", "F8 (modo ratón) y F11 (pantalla completa) están reservadas por chiaki-ng.", "F8 (modo mouse) e F11 (tela cheia) são reservadas pelo chiaki-ng.", "F8 (mode souris) et F11 (plein écran) sont réservées par chiaki-ng.", "F8 (Mausmodus) und F11 (Vollbild) sind von chiaki-ng belegt.", "F8 (режим мыши) и F11 (полный экран) заняты chiaki-ng.", "F8（鼠标模式）和 F11（全屏）为 chiaki-ng 保留。" } },
			{ "unsupported", new[] { "This key can't be used (only A–Z, numbers and common keys; no accented letters or punctuation).", "Bu tuş kullanılamıyor (yalnızca A–Z, rakamlar ve yaygın tuşlar; Türkçe harfler ve noktalama yok).", "Esta tecla no se puede usar (solo A–Z, números y teclas comunes; sin letras acentuadas ni puntuación).", "Esta tecla não pode ser usada (só A–Z, números e teclas comuns; sem acentos ou pontuação).", "Touche non utilisable (seulement A–Z, chiffres et touches courantes ; pas d'accents ni de ponctuation).", "Diese Taste geht nicht (nur A–Z, Ziffern und gängige Tasten; keine Umlaute oder Satzzeichen).", "Эту клавишу нельзя назначить (только A–Z, цифры и обычные клавиши; без букв с диакритикой и знаков).", "该键不可用（仅支持 A–Z、数字和常用键，不支持标点）。" } },
			{ "ctrl_note", new[] { "Note: in chiaki-ng, Ctrl+Q closes the app.", "Not: chiaki-ng'de Ctrl+Q programı kapatır.", "Nota: en chiaki-ng, Ctrl+Q cierra la aplicación.", "Nota: no chiaki-ng, Ctrl+Q fecha o app.", "Remarque : dans chiaki-ng, Ctrl+Q ferme l'application.", "Hinweis: In chiaki-ng beendet Strg+Q die App.", "Внимание: в chiaki-ng Ctrl+Q закрывает программу.", "注意：在 chiaki-ng 中 Ctrl+Q 会关闭程序。" } },
			{ "defaults_loaded", new[] { "Defaults loaded (not saved yet).", "Varsayılanlar yüklendi (henüz kaydedilmedi).", "Valores predeterminados cargados (sin guardar).", "Padrões carregados (ainda não salvos).", "Valeurs par défaut chargées (non enregistrées).", "Standard geladen (noch nicht gespeichert).", "Загружены настройки по умолчанию (не сохранены).", "已载入默认设置（尚未保存）。" } },
			{ "first_run", new[] { "Keys loaded from the game's default layout. Check them and press START.", "Oyunun varsayılan düzenine göre tuşlar yüklendi. Kontrol edip BAŞLAT'a basın.", "Teclas cargadas según el esquema predeterminado del juego. Revísalas y pulsa INICIAR.", "Teclas carregadas do esquema padrão do jogo. Confira e clique em INICIAR.", "Touches chargées d'après le schéma par défaut du jeu. Vérifiez puis cliquez sur LANCER.", "Tasten nach dem Standardschema des Spiels geladen. Prüfen und STARTEN drücken.", "Клавиши загружены по стандартной схеме игры. Проверьте и нажмите ЗАПУСК.", "已按游戏默认布局载入按键。检查后点击启动。" } },
			{ "save_failed", new[] { "Couldn't save the settings:\n", "Ayarlar kaydedilemedi:\n", "No se pudo guardar la configuración:\n", "Não foi possível salvar:\n", "Impossible d'enregistrer :\n", "Speichern fehlgeschlagen:\n", "Не удалось сохранить настройки:\n", "无法保存设置：\n" } },
			{ "exe_missing", new[] { "chiaki.exe not found:\n", "chiaki.exe bulunamadı:\n", "No se encontró chiaki.exe:\n", "chiaki.exe não encontrado:\n", "chiaki.exe introuvable :\n", "chiaki.exe nicht gefunden:\n", "chiaki.exe не найден:\n", "未找到 chiaki.exe：\n" } },
			{ "already_open", new[] {
				"chiaki-ng is already open. New settings only apply after it restarts.\n\nClose it and start again?",
				"chiaki-ng zaten açık. Yeni ayarlar ancak chiaki-ng yeniden açılınca geçerli olur.\n\nAçık olanı kapatıp yeniden başlatayım mı?",
				"chiaki-ng ya está abierto. La nueva configuración se aplica al reiniciarlo.\n\n¿Cerrarlo y volver a iniciarlo?",
				"O chiaki-ng já está aberto. As novas configurações só valem após reiniciá-lo.\n\nFechar e iniciar de novo?",
				"chiaki-ng est déjà ouvert. Les nouveaux réglages s'appliquent après redémarrage.\n\nLe fermer et relancer ?",
				"chiaki-ng läuft bereits. Neue Einstellungen gelten erst nach einem Neustart.\n\nSchließen und neu starten?",
				"chiaki-ng уже открыт. Новые настройки применятся после перезапуска.\n\nЗакрыть и запустить заново?",
				"chiaki-ng 已在运行。新设置需重启后生效。\n\n关闭并重新启动？" } },
		};
	}
}
