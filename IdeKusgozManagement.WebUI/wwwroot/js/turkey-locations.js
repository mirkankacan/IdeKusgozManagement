// Turkish Provinces and Districts Data - Complete List
const turkeyLocations = {
    "35": {
        name: "İzmir",
        districts: ["Aliağa", "Bayındır", "Bergama", "Bornova", "Çeşme", "Dikili", "Foça", "Karaburun", "Karşıyaka", "Kemalpaşa", "Kınık", "Kiraz", "Menemen", "Ödemiş", "Seferihisar", "Selçuk", "Tire", "Torbalı", "Urla", "Beydağ", "Menderes", "Balçova", "Çiğli", "Gaziemir", "Güzelbahçe", "Konak", "Narlıdere", "Karabağlar", "Bayraklı"]
    },
    "34": {
        name: "İstanbul",
        districts: ["Adalar", "Arnavutköy", "Ataşehir", "Avcılar", "Bağcılar", "Bahçelievler", "Bakırköy", "Başakşehir", "Bayrampaşa", "Beşiktaş", "Beykoz", "Beylikdüzü", "Beyoğlu", "Büyükçekmece", "Çatalca", "Çekmeköy", "Esenler", "Esenyurt", "Eyüpsultan", "Fatih", "Gaziosmanpaşa", "Güngören", "Kadıköy", "Kağıthane", "Kartal", "Küçükçekmece", "Maltepe", "Pendik", "Sancaktepe", "Sarıyer", "Silivri", "Sultanbeyli", "Sultangazi", "Şile", "Şişli", "Tuzla", "Ümraniye", "Üsküdar", "Zeytinburnu"]
    },
    "06": {
        name: "Ankara",
        districts: ["Akyurt", "Altındağ", "Ayaş", "Bala", "Beypazarı", "Çamlıdere", "Çankaya", "Çubuk", "Elmadağ", "Etimesgut", "Evren", "Gölbaşı", "Güdül", "Haymana", "Kalecik", "Kazan", "Keçiören", "Kızılcahamam", "Mamak", "Nallıhan", "Polatlı", "Pursaklar", "Sincan", "Şereflikoçhisar", "Yenimahalle"]
    },
    "01": {
        name: "Adana",
        districts: ["Aladağ", "Ceyhan", "Çukurova", "Feke", "İmamoğlu", "Karaisalı", "Karataş", "Kozan", "Pozantı", "Saimbeyli", "Sarıçam", "Seyhan", "Tufanbeyli", "Yumurtalık", "Yüreğir"]
    },
    "02": {
        name: "Adıyaman",
        districts: ["Besni", "Çelikhan", "Gerger", "Gölbaşı", "Kahta", "Merkez", "Samsat", "Sincik", "Tut"]
    },
    "03": {
        name: "Afyonkarahisar",
        districts: ["Başmakçı", "Bayat", "Bolvadin", "Çay", "Çobanlar", "Dazkırı", "Dinar", "Emirdağ", "Evciler", "Hocalar", "İhsaniye", "İscehisar", "Kızılören", "Merkez", "Sandıklı", "Sinanpaşa", "Sultandağı", "Şuhut"]
    },
    "04": {
        name: "Ağrı",
        districts: ["Diyadin", "Doğubayazıt", "Eleşkirt", "Hamur", "Merkez", "Patnos", "Taşlıçay", "Tutak"]
    },
    "05": {
        name: "Amasya",
        districts: ["Göynücek", "Gümüşhacıköy", "Hamamözü", "Merkez", "Merzifon", "Suluova", "Taşova"]
    },
    "07": {
        name: "Antalya",
        districts: ["Akseki", "Aksu", "Alanya", "Demre", "Döşemealtı", "Elmalı", "Finike", "Gazipaşa", "Gündoğmuş", "İbradı", "Kaş", "Kemer", "Kepez", "Konyaaltı", "Korkuteli", "Kumluca", "Manavgat", "Muratpaşa", "Serik"]
    },
    "08": {
        name: "Artvin",
        districts: ["Ardanuç", "Arhavi", "Merkez", "Borçka", "Hopa", "Şavşat", "Yusufeli"]
    },
    "09": {
        name: "Aydın",
        districts: ["Bozdoğan", "Çine", "Germencik", "Karacasu", "Koçarlı", "Kuşadası", "Kuyucak", "Merkez", "Nazilli", "Söke", "Sultanhisar", "Yenipazar"]
    },
    "10": {
        name: "Balıkesir",
        districts: ["Altıeylül", "Ayvalık", "Balya", "Bandırma", "Bigadiç", "Burhaniye", "Dursunbey", "Edremit", "Erdek", "Gönen", "Havran", "İvrindi", "Karesi", "Kepsut", "Manyas", "Savaştepe", "Sındırgı", "Gömeç", "Susurluk", "Marmara"]
    },
    "11": {
        name: "Bilecik",
        districts: ["Bozüyük", "Gölpazarı", "Merkez", "Osmaneli", "Pazaryeri", "Söğüt", "Yenipazar", "İnhisar"]
    },
    "12": {
        name: "Bingöl",
        districts: ["Genç", "Karlıova", "Kiğı", "Merkez", "Solhan", "Adaklı", "Yayladere", "Yedisu"]
    },
    "13": {
        name: "Bitlis",
        districts: ["Adilcevaz", "Ahlat", "Hizan", "Merkez", "Mutki", "Tatvan", "Güroymak"]
    },
    "14": {
        name: "Bolu",
        districts: ["Gerede", "Göynük", "Kıbrıscık", "Mengen", "Merkez", "Mudurnu", "Seben", "Dörtdivan", "Yeniçağa"]
    },
    "15": {
        name: "Burdur",
        districts: ["Ağlasun", "Bucak", "Gölhisar", "Tefenni", "Yeşilova", "Merkez", "Çavdır", "Çeltikçi"]
    },
    "16": {
        name: "Bursa",
        districts: ["Gemlik", "İnegöl", "İznik", "Karacabey", "Keles", "Mudanya", "Mustafakemalpaşa", "Orhaneli", "Orhangazi", "Yenişehir", "Büyükorhan", "Harmancık", "Nilüfer", "Osmangazi", "Yıldırım", "Gürsu", "Kestel"]
    },
    "17": {
        name: "Çanakkale",
        districts: ["Ayvacık", "Bayramiç", "Biga", "Bozcaada", "Çan", "Eceabat", "Ezine", "Gelibolu", "Gökçeada", "Lapseki", "Merkez", "Yenice"]
    },
    "18": {
        name: "Çankırı",
        districts: ["Çerkeş", "Eldivan", "Ilgaz", "Kurşunlu", "Orta", "Şabanözü", "Atkaracalar", "Kızılırmak", "Bayramören", "Korgun", "Yapraklı"]
    },
    "19": {
        name: "Çorum",
        districts: ["Alaca", "Bayat", "İskilip", "Kargı", "Mecitözü", "Merkez", "Ortaköy", "Osmancık", "Sungurlu", "Boğazkale", "Uğurludağ", "Dodurga", "Laçin", "Oğuzlar"]
    },
    "20": {
        name: "Denizli",
        districts: ["Acıpayam", "Buldan", "Çal", "Çameli", "Çardak", "Fethiye", "Güney", "Kale", "Merkez", "Sarayköy", "Tavas", "Babadağ", "Bekilli", "Honaz", "Serinhisar", "Pamukkale", "Baklan", "Beyağaç", "Bozkurt"]
    },
    "21": {
        name: "Diyarbakır",
        districts: ["Bismil", "Çermik", "Ergani", "Hani", "Hazro", "Kulp", "Lice", "Silvan", "Merkez", "Eğil", "Kocaköy", "Çüngüş", "Dicle", "Bağlar", "Kayapınar", "Sur", "Yenişehir"]
    },
    "22": {
        name: "Edirne",
        districts: ["Enez", "Havsa", "İpsala", "Keşan", "Lalapaşa", "Meriç", "Uzunköprü", "Merkez", "Süloğlu"]
    },
    "23": {
        name: "Elazığ",
        districts: ["Ağın", "Baskil", "Karakoçan", "Keban", "Maden", "Merkez", "Palu", "Sivrice", "Arıcak", "Kovancılar", "Alacakaya"]
    },
    "24": {
        name: "Erzincan",
        districts: ["Çayırlı", "İliç", "Kemah", "Kemaliye", "Refahiye", "Tercan", "Üzümlü", "Merkez", "Otlukbeli"]
    },
    "25": {
        name: "Erzurum",
        districts: ["Aşkale", "Çat", "Hınıs", "Horasan", "İspir", "Karayazı", "Narman", "Oltu", "Olur", "Pasinler", "Şenkaya", "Tekman", "Tortum", "Karaçoban", "Uzundere", "Pazaryolu", "Köprüköy", "Hınıs", "Aziziye", "Yakutiye", "Palandöken"]
    },
    "26": {
        name: "Eskişehir",
        districts: ["Çifteler", "Mahmudiye", "Mihalıççık", "Sarıcakaya", "Seyitgazi", "Sivrihisar", "Alpu", "Beylikova", "Günyüzü", "Han", "İnönü", "Mihalgazi", "Odunpazarı", "Tepebaşı"]
    },
    "27": {
        name: "Gaziantep",
        districts: ["Araban", "İslahiye", "Nizip", "Oğuzeli", "Yavuzeli", "Şahinbey", "Şehitkamil", "Karkamış", "Nurdağı"]
    },
    "28": {
        name: "Giresun",
        districts: ["Alucra", "Bulancak", "Dereli", "Espiye", "Eynesil", "Görele", "Keşap", "Şebinkarahisar", "Tirebolu", "Piraziz", "Yağlıdere", "Çanakçı", "Doğankent", "Güce"]
    },
    "29": {
        name: "Gümüşhane",
        districts: ["Kelkit", "Şiran", "Torul", "Merkez", "Köse", "Kürtün"]
    },
    "30": {
        name: "Hakkari",
        districts: ["Çukurca", "Şemdinli", "Yüksekova", "Merkez", "Derecik"]
    },
    "31": {
        name: "Hatay",
        districts: ["Altınözü", "Dörtyol", "Hassa", "İskenderun", "Kırıkhan", "Reyhanlı", "Samandağ", "Yayladağı", "Erzin", "Belen", "Kumlu", "Defne", "Payas"]
    },
    "32": {
        name: "Isparta",
        districts: ["Atabey", "Eğirdir", "Gelendost", "Keçiborlu", "Senirkent", "Sütçüler", "Şarkikaraağaç", "Uluborlu", "Yalvaç", "Aksu", "Gönen", "Yenişarbademli"]
    },
    "33": {
        name: "Mersin",
        districts: ["Anamur", "Erdemli", "Gülnar", "Mut", "Silifke", "Tarsus", "Aydıncık", "Bozyazı", "Çamlıyayla", "Mezitli", "Toroslar", "Yenişehir"]
    },
    "36": {
        name: "Kars",
        districts: ["Arpaçay", "Digor", "Kağızman", "Merkez", "Sarıkamış", "Selim", "Susuz", "Akyaka"]
    },
    "37": {
        name: "Kastamonu",
        districts: ["Araç", "Cide", "İnebolu", "Küre", "Taşköprü", "Tosya", "Azdavay", "Bozkurt", "Çatalzeytin", "Daday", "Devrekani", "İhsangazi", "Seydiler", "Şenpazar", "Abana", "Doğanyurt", "Hanönü", "Pınarbaşı", "Seydiler", "Ağlı", "Doğanyurt", "Hanönü", "Pınarbaşı"]
    },
    "38": {
        name: "Kayseri",
        districts: ["Bünyan", "Develi", "Felahiye", "İncesu", "Pınarbaşı", "Sarıoğlan", "Sarız", "Tomarza", "Yahyalı", "Yeşilhisar", "Akkışla", "Hacılar", "Özvatan", "Kocasinan", "Melikgazi", "Talas"]
    },
    "39": {
        name: "Kırklareli",
        districts: ["Babaeski", "Demirköy", "Merkez", "Pehlivanköy", "Pınarhisar", "Vize", "Kofçaz", "Lüleburgaz"]
    },
    "40": {
        name: "Kırşehir",
        districts: ["Çiçekdağı", "Kaman", "Mucur", "Merkez", "Akpınar", "Akçakent", "Boztepe"]
    },
    "41": {
        name: "Kocaeli",
        districts: ["Gebze", "Gölcük", "Kandıra", "Karamürsel", "Körfez", "Derince", "Başiskele", "Çayırova", "Darıca", "Dilovası", "İzmit", "Kartepe"]
    },
    "42": {
        name: "Konya",
        districts: ["Akşehir", "Beyşehir", "Bozkır", "Cihanbeyli", "Çumra", "Doğanhisar", "Ereğli", "Hadim", "Ilgın", "Kadınhanı", "Karapınar", "Kulu", "Sarayönü", "Seydişehir", "Yunak", "Akören", "Altınekin", "Derebucak", "Emirgazi", "Güneysınır", "Halkapınar", "Hüyük", "Karataş", "Meram", "Selçuklu", "Taşkent", "Tuzlukçu", "Yalıhüyük"]
    },
    "43": {
        name: "Kütahya",
        districts: ["Altıntaş", "Domaniç", "Emet", "Gediz", "Simav", "Tavşanlı", "Aslanapa", "Dumlupınar", "Hisarcık", "Şaphane", "Çavdarhisar", "Pazarlar", "Banaz", "Sivaslı", "Karahallı", "Ulubey", "Eşme", "Karahallı", "Banaz", "Sivaslı", "Ulubey"]
    },
    "44": {
        name: "Malatya",
        districts: ["Akçadağ", "Arapgir", "Arguvan", "Darende", "Doğanşehir", "Hekimhan", "Pütürge", "Yeşilyurt", "Battalgazi", "Doğanyol", "Kale", "Kuluncuk", "Yazıhan"]
    },
    "45": {
        name: "Manisa",
        districts: ["Akhisar", "Alaşehir", "Demirci", "Gördes", "Kırkağaç", "Kula", "Salihli", "Sarıgöl", "Saruhanlı", "Turgutlu", "Akhisar", "Alaşehir", "Demirci", "Gördes", "Kırkağaç", "Kula", "Salihli", "Sarıgöl", "Saruhanlı", "Turgutlu", "Ahmetli", "Gölmarmara", "Köprübaşı", "Şehzadeler", "Yunusemre"]
    },
    "46": {
        name: "Kahramanmaraş",
        districts: ["Afşin", "Andırın", "Elbistan", "Göksun", "Pazarcık", "Türkoğlu", "Çağlayancerit", "Ekinözü", "Nurhak", "Dulkadiroğlu", "Onikişubat"]
    },
    "47": {
        name: "Mardin",
        districts: ["Derik", "Kızıltepe", "Mazıdağı", "Midyat", "Nusaybin", "Ömerli", "Savur", "Yeşilli", "Dargeçit", "Artuklu"]
    },
    "48": {
        name: "Muğla",
        districts: ["Bodrum", "Datça", "Fethiye", "Köyceğiz", "Marmaris", "Menteşe", "Milas", "Ula", "Yatağan", "Dalaman", "Ortaca", "Kavaklıdere", "Seydikemer"]
    },
    "49": {
        name: "Muş",
        districts: ["Bulanık", "Malazgirt", "Merkez", "Varto", "Hasköy", "Korkut"]
    },
    "50": {
        name: "Nevşehir",
        districts: ["Avanos", "Derinkuyu", "Gülşehir", "Hacıbektaş", "Kozaklı", "Ürgüp", "Acıgöl", "Merkez"]
    },
    "51": {
        name: "Niğde",
        districts: ["Aksaray", "Bor", "Çamardı", "Ulukışla", "Merkez", "Altunhisar", "Çiftlik"]
    },
    "52": {
        name: "Ordu",
        districts: ["Akkuş", "Aybastı", "Fatsa", "Gölköy", "Korgan", "Kumru", "Mesudiye", "Perşembe", "Ulubey", "Ünye", "Gürgentepe", "Çamaş", "Çatalpınar", "Çaybaşı", "İkizce", "Kabadüz", "Kabataş", "Kabadüz", "Kabataş", "İkizce", "Çaybaşı", "Çatalpınar", "Çamaş", "Gürgentepe"]
    },
    "53": {
        name: "Rize",
        districts: ["Ardeşen", "Çamlıhemşin", "Çayeli", "Fındıklı", "İkizdere", "Kalkandere", "Pazar", "Merkez", "Güneysu", "Derepazarı", "Hemşin", "İyidere"]
    },
    "54": {
        name: "Sakarya",
        districts: ["Akyazı", "Geyve", "Hendek", "Karasu", "Kaynarca", "Sapanca", "Kocaali", "Pamukova", "Taraklı", "Ferizli", "Karapürçek", "Söğütlü", "Adapazarı", "Arifiye", "Erenler", "Serdivan"]
    },
    "55": {
        name: "Samsun",
        districts: ["Alaçam", "Bafra", "Çarşamba", "Havza", "Kavak", "Ladik", "Terme", "Vezirköprü", "Asarcık", "Ondokuzmayıs", "Salıpazarı", "Tekkeköy", "Ayvacık", "Yakakent", "19 Mayıs", "Alaçam", "Bafra", "Çarşamba", "Havza", "Kavak", "Ladik", "Terme", "Vezirköprü", "Asarcık", "Ondokuzmayıs", "Salıpazarı", "Tekkeköy", "Ayvacık", "Yakakent", "19 Mayıs"]
    },
    "56": {
        name: "Siirt",
        districts: ["Baykan", "Eruh", "Kurtalan", "Pervari", "Şirvan", "Merkez", "Tillo"]
    },
    "57": {
        name: "Sinop",
        districts: ["Ayancık", "Boyabat", "Durağan", "Erfelek", "Gerze", "Türkeli", "Merkez", "Saraydüzü", "Dikmen"]
    },
    "58": {
        name: "Sivas",
        districts: ["Divriği", "Gemerek", "Hafik", "İmranlı", "Kangal", "Koyulhisar", "Şarkışla", "Suşehri", "Zara", "Akıncılar", "Altınyayla", "Doğanşar", "Gölova", "Ulaş", "Yıldızeli", "Merkez"]
    },
    "59": {
        name: "Tekirdağ",
        districts: ["Çerkezköy", "Çorlu", "Ergene", "Hayrabolu", "Malkara", "Muratlı", "Saray", "Süleymanpaşa", "Şarköy", "Kapaklı", "Marmaraereğlisi"]
    },
    "60": {
        name: "Tokat",
        districts: ["Almus", "Artova", "Erbaa", "Niksar", "Reşadiye", "Turhal", "Zile", "Pazar", "Yeşilyurt", "Başçiftlik", "Sulusaray"]
    },
    "61": {
        name: "Trabzon",
        districts: ["Akçaabat", "Araklı", "Arsin", "Çaykara", "Maçka", "Of", "Sürmene", "Tonya", "Vakfıkebir", "Yomra", "Beşikdüzü", "Şalpazarı", "Çarşıbaşı", "Dernekpazarı", "Düzköy", "Hayrat", "Köprübaşı", "Ortahisar"]
    },
    "62": {
        name: "Tunceli",
        districts: ["Çemişgezek", "Hozat", "Mazgirt", "Nazımiye", "Ovacık", "Pertek", "Pülümür", "Merkez"]
    },
    "63": {
        name: "Şanlıurfa",
        districts: ["Akçakale", "Birecik", "Bozova", "Ceylanpınar", "Halfeti", "Hilvan", "Siverek", "Suruç", "Viranşehir", "Harran", "Eyyübiye", "Haliliye", "Karaköprü"]
    },
    "64": {
        name: "Uşak",
        districts: ["Banaz", "Eşme", "Karahallı", "Sivaslı", "Ulubey", "Merkez"]
    },
    "65": {
        name: "Van",
        districts: ["Başkale", "Çatak", "Erciş", "Gevaş", "Gürpınar", "Muradiye", "Özalp", "Bahçesaray", "Çaldıran", "Edremit", "Saray", "Tuşba", "İpekyolu"]
    },
    "66": {
        name: "Yozgat",
        districts: ["Akdağmadeni", "Boğazlıyan", "Sarıkaya", "Şefaatli", "Yerköy", "Çayıralan", "Çekerek", "Sorgun", "Aydıncık", "Kadışehri", "Saraykent", "Yenifakılı", "Şefaatli", "Yerköy", "Çayıralan", "Çekerek", "Sorgun", "Aydıncık", "Kadışehri", "Saraykent", "Yenifakılı"]
    },
    "67": {
        name: "Zonguldak",
        districts: ["Çaycuma", "Devrek", "Ereğli", "Merkez", "Alaplı", "Gökçebey", "Kilimli", "Kozlu"]
    },
    "68": {
        name: "Aksaray",
        districts: ["Merkez", "Ortaköy", "Ağaçören", "Güzelyurt", "Sarıyahşi", "Eskil", "Gülağaç"]
    },
    "69": {
        name: "Bayburt",
        districts: ["Merkez", "Aydıntepe", "Demirözü"]
    },
    "70": {
        name: "Karaman",
        districts: ["Ermenek", "Merkez", "Ayrancı", "Kazımkarabekir", "Başyayla", "Sarıveliler"]
    },
    "71": {
        name: "Kırıkkale",
        districts: ["Delice", "Keskin", "Sulakyurt", "Bahşılı", "Balışeyh", "Çelebi", "Karakeçili", "Yahşihan", "Merkez"]
    },
    "72": {
        name: "Batman",
        districts: ["Beşiri", "Gercüş", "Kozluk", "Merkez", "Hasankeyf", "Sason"]
    },
    "73": {
        name: "Şırnak",
        districts: ["Beytüşşebap", "Cizre", "İdil", "Silopi", "Merkez", "Uludere", "Güçlükonak"]
    },
    "74": {
        name: "Bartın",
        districts: ["Merkez", "Kurucaşile", "Ulus", "Amasra"]
    },
    "75": {
        name: "Ardahan",
        districts: ["Göle", "Hanak", "Merkez", "Posof", "Çıldır", "Damal"]
    },
    "76": {
        name: "Iğdır",
        districts: ["Aralık", "Merkez", "Tuzluca", "Karakoyunlu"]
    },
    "77": {
        name: "Yalova",
        districts: ["Merkez", "Altınova", "Armutlu", "Çınarcık", "Çiftlikköy", "Termal"]
    },
    "78": {
        name: "Karabük",
        districts: ["Eflani", "Eskipazar", "Merkez", "Ovacık", "Safranbolu", "Yenice"]
    },
    "79": {
        name: "Kilis",
        districts: ["Merkez", "Elbeyli", "Musabeyli", "Polateli"]
    },
    "80": {
        name: "Osmaniye",
        districts: ["Bahçe", "Kadirli", "Merkez", "Düziçi", "Hasanbeyli", "Sumbas", "Toprakkale"]
    },
    "81": {
        name: "Düzce",
        districts: ["Akçakoca", "Merkez", "Yığılca", "Cumayeri", "Gölyaka", "Çilimli", "Gümüşova", "Kaynaşlı"]
    }
};

// Function to populate province select
function populateProvinces(selectElement) {
    selectElement.empty();
    selectElement.append('<option value="" selected disabled>İl seçin</option>');

    // Öncelikli iller (İzmir, İstanbul, Ankara)
    const priorityProvinces = ["35", "34", "06"];
    
    // Önce öncelikli illeri ekle
    priorityProvinces.forEach(code => {
        if (turkeyLocations[code]) {
            const province = turkeyLocations[code];
            selectElement.append(`<option value="${province.name}" data-code="${code}">${province.name}</option>`);
        }
    });
    
    // Sonra kalan illeri plaka koduna göre sıralı ekle
    Object.keys(turkeyLocations)
        .filter(code => !priorityProvinces.includes(code))
        .sort((a, b) => parseInt(a) - parseInt(b))
        .forEach(code => {
            const province = turkeyLocations[code];
            selectElement.append(`<option value="${province.name}" data-code="${code}">${province.name}</option>`);
        });
}

// Function to populate districts based on selected province
function populateDistricts(provinceSelect, districtSelect) {
    const selectedProvinceCode = provinceSelect.find('option:selected').data('code');
    districtSelect.empty();
    districtSelect.append('<option value="" selected disabled>İlçe seçin</option>');

    if (selectedProvinceCode && turkeyLocations[selectedProvinceCode]) {
        const districts = turkeyLocations[selectedProvinceCode].districts;
        districts.forEach(district => {
            districtSelect.append(`<option value="${district}">${district}</option>`);
        });
    }
}
