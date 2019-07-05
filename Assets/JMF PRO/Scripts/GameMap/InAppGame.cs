using UnityEngine;
using System.Collections;
using UnityEngine.Purchasing;
using System;
using UnityEngine.Purchasing.Extension;

public class InAppGame : MonoBehaviour, IStoreListener
{
    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;

    public static string kProductIDConsumable099 = "do099";
    public static string kProductIDConsumable299 = "do2991";
    public static string kProductIDConsumable499 = "do499";
    public static string kProductIDConsumable999 = "do999";
    public static string kProductIDConsumable4999 = "do4999";
    public static string kProductIDConsumable9999 = "do9999";
    // Use this for initialization

    private static string kProductNameAppleConsumable = "com.unity3d.test.services.purchasing.consumable";

    //private static string kProductNameGooglePlayConsumable = "com.fruit.fram.puzzle.panda";

    void Start()
    {
        if (m_StoreController == null)
        {
            InitializePurchasing();
        }
    }

    private IStoreCallback callback;
    public void Initialize(IStoreCallback callback)
    {
        this.callback = callback;
    }

    public void RetrieveProducts(System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Purchasing.ProductDefinition> products)
    {
        // Fetch product information and invoke callback.OnProductsRetrieved();
    }

    public void Purchase(UnityEngine.Purchasing.ProductDefinition product, string developerPayload)
    {
        // Start the purchase flow and call either callback.OnPurchaseSucceeded() or callback.OnPurchaseFailed()
    }

    public void FinishTransaction(UnityEngine.Purchasing.ProductDefinition product, string transactionId)
    {
        // Perform transaction related housekeeping 
    }
    public void InitializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }
        var module = StandardPurchasingModule.Instance();
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);
        builder.AddProduct(kProductIDConsumable099, ProductType.Consumable);
        builder.AddProduct(kProductIDConsumable299, ProductType.Consumable);
        builder.AddProduct(kProductIDConsumable499, ProductType.Consumable);
        builder.AddProduct(kProductIDConsumable999, ProductType.Consumable);
        builder.AddProduct(kProductIDConsumable4999, ProductType.Consumable);
        builder.AddProduct(kProductIDConsumable9999, ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);
    }
    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void BuyProductID(string productId)
    {
        try
        {
            if (IsInitialized())
            {
                Product product = m_StoreController.products.WithID(productId);

                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    m_StoreController.InitiatePurchase(product);
                }
                else
                {
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            else
            {
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }
        catch (Exception e)
        {
            Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
        }
    }


    public void RestorePurchases()
    {
        if (!IsInitialized())
        {
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("RestorePurchases started ...");

            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((result) =>
            {
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        else
        {
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized: PASS");

        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        int addCoin = 0;
        if (String.Equals(args.purchasedProduct.definition.id, kProductIDConsumable099, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            addCoin = 10000;
        }
        else if (String.Equals(args.purchasedProduct.definition.id, kProductIDConsumable299, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            addCoin = 30000;
        }
        else if (String.Equals(args.purchasedProduct.definition.id, kProductIDConsumable499, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            addCoin = 60000;
        }
        else if (String.Equals(args.purchasedProduct.definition.id, kProductIDConsumable999, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            addCoin = 130000;
        }

        else if (String.Equals(args.purchasedProduct.definition.id, kProductIDConsumable4999, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            addCoin = 7000000;
        }
        else if (String.Equals(args.purchasedProduct.definition.id, kProductIDConsumable9999, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            addCoin = 15000000;
        }

        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }

        int totalCoin = PlayerPrefs.GetInt("totalcoin");
        totalCoin += addCoin;
        Debug.Log(totalCoin);
        PlayerPrefs.SetInt("totalcoin", totalCoin);
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
}
