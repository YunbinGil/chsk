// using System.Linq;
// using TMPro;
// using UnityEngine;

// // buy_btn 오브젝트에 붙여서 사용
// // 같은 셀 안의 가격 텍스트("Text.price.gold")를 찾아 숫자 파싱 → GoldBuyBtn.price에 세팅
// // [RequireComponent(typeof(GoldBuyBtn))]
// public class ShopCellPriceBinder : MonoBehaviour
// {
//     [Header("가격 텍스트(비워두면 이름으로 자동 탐색)")]
//     [SerializeField] private TMP_Text priceText;

//     [Tooltip("자동 탐색 시 찾을 오브젝트 이름")]
//     [SerializeField] private string priceTextName = "Text.price.gold";

//     [Header("옵션")]
//     [Tooltip("Awake에서 자동 바인딩할지")]
//     [SerializeField] private bool bindOnAwake = true;

//     // private GoldBuyBtn _buyBtn;

//     void Reset()
//     {
//         // _buyBtn = GetComponent<GoldBuyBtn>();
//         if (!priceText) priceText = FindPriceLabel();
//         // TryBind();
//     }

//     void Awake()
//     {
//         // if (_buyBtn == null) _buyBtn = GetComponent<GoldBuyBtn>();
//         // if (bindOnAwake) TryBind();
//     }

//     // 에디터에서 값 바뀔 때도 자동으로 잡히게
// #if UNITY_EDITOR
//     void OnValidate()
//     {
//         // if (_buyBtn == null) _buyBtn = GetComponent<GoldBuyBtn>();
//         if (!Application.isPlaying && priceText == null)
//             priceText = FindPriceLabel();
//     }
// #endif

//     public void TryBind()
//     {
//         if (priceText == null)
//             priceText = FindPriceLabel();

//         if (_buyBtn == null)
//             _buyBtn = GetComponent<GoldBuyBtn>();

//         int parsed = ParsePrice(priceText ? priceText.text : null);
//         _buyBtn.price = Mathf.Max(0, parsed);
//         // Debug.Log($"[ShopCellPriceBinder] {_buyBtn.name} price = {_buyBtn.price}");
//     }

//     // 같은 셀(부모 트리) 안에서 이름으로 텍스트 찾기
//     private TMP_Text FindPriceLabel()
//     {
//         // buy_btn → 상위로 올라가서 모든 자식 중 name 매칭
//         var root = transform.parent ? transform.parent : transform;
//         var texts = root.GetComponentsInChildren<TMP_Text>(true);
//         return texts.FirstOrDefault(t => t.gameObject.name == priceTextName);
//     }

//     // "1,200 G", "x 300", "300골드", "₲ 2,000" 같은 문자열에서 숫자만 추출
//     private int ParsePrice(string s)
//     {
//         if (string.IsNullOrEmpty(s)) return 0;

//         // 숫자만 추출
//         string digits = new string(s.Where(char.IsDigit).ToArray());
//         if (int.TryParse(digits, out int v)) return v;

//         return 0;
//     }
// }
