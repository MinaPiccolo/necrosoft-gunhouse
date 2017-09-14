using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SonyNpCommerceInGameStore : IScreen
{
	// A note on commerce sessions...
	//
	// We must create a commerce session before calling RequestCategoryInfo(), RequestProductList(), or RequestDetailedProductInfo()
	//
	// Commerce session are created by calling Sony.NP.Commerce.CreateSession().
	//
	// Commerce sessions can expire on the server after 15+ minutes of inactivity (SDK docs states that the time
	// can vary) so it is important to check the error code in your Sony.NP.Commerce.OnError delegate handler and
	// if they fail with Sony.NP.ErrorCode.NP_ERR_COMMERCE_SESSION_EXPIRED then call CreateSession before retrying.
	//
	// To make it easy to retry after an asynchronous failure caused by session expiration we use the IRequest derived
	// classes defined below to wrap requests that require a valid commerce session...

	abstract class IRequest
	{
		abstract public Sony.NP.ErrorCode DoRequest();
	}

	class RequestCategoryInfo : IRequest
	{
		public RequestCategoryInfo(string categoryID)
		{
			m_CategoryId = categoryID;
		}
		public override Sony.NP.ErrorCode DoRequest()
		{
			return Sony.NP.Commerce.RequestCategoryInfo(m_CategoryId);
		}

		string m_CategoryId;
	}

	class RequestProductList : IRequest
	{
		public RequestProductList(string categoryID)
		{
			m_CategoryId = categoryID;
		}
		public override Sony.NP.ErrorCode DoRequest()
		{
			return Sony.NP.Commerce.RequestProductList(m_CategoryId);
		}

		string m_CategoryId;
	}

	class RequestDetailedProductInfo : IRequest
	{
		public RequestDetailedProductInfo(string productID)
		{
			m_ProductID = productID;
		}

		public override Sony.NP.ErrorCode DoRequest()
		{
			return Sony.NP.Commerce.RequestDetailedProductInfo(m_ProductID);
		}

		string m_ProductID;
	}

	class RequestCheckout : IRequest
	{
		public RequestCheckout(string[] productSkuIDs)
		{
			m_ProductSkuIDs = productSkuIDs;
		}

		public override Sony.NP.ErrorCode DoRequest()
		{
			return Sony.NP.Commerce.Checkout(m_ProductSkuIDs);
		}

		string[] m_ProductSkuIDs;
	}

	IRequest m_PendingRequest = null;
	bool m_SessionCreated = false;
	MenuLayout m_Menu;

#if UNITY_PSP2
	string m_TestCategoryID = "ED1633-NPXB01864_00-WEAPS_01";
	string m_TestProductID = "ED1633-NPXB01864_00-A000010000000000";
	string[] m_TestProductSkuIDs = new string[] { "ED1633-NPXB01864_00-A000010000000000-E001", "ED1633-NPXB01864_00-A000020000000000-E001" };
#elif UNITY_PS3
	// TODO: These should be changed to use the PS3 IDs.
	string m_TestCategoryID = "ED1633-NPXB01864_00-WEAPS_01";
	string m_TestProductID = "ED1633-NPXB01864_00-A000010000000000";
	string[] m_TestProductSkuIDs = new string[] { "ED1633-NPXB01864_00-A000010000000000-E001", "ED1633-NPXB01864_00-A000020000000000-E001" };
#elif UNITY_PS4
    // On the PS4 the company/product id (e.g. ED1633-NPXX51362_00) needs to be removed from any product ID, category ID etc. 
    string m_TestCategoryID = "WEAPS_01";         // We haven't created any sub-category's for the npToolkit test store so this will just return an error. Remove the company / product perfix from the CategoryID 
    string m_TestProductID = "A000010000000000";  // PS4 requires product ID's without all the company / product stuff
    string[] m_TestProductSkuIDs = new string[] { "A000010000000000-E001", "A000020000000000-E001" };
#else
	string m_TestCategoryID = "";
	string m_TestProductID = "";
	string[] m_TestProductSkuIDs = new string[] { "" };
#endif

	public SonyNpCommerceInGameStore()
	{
		Initialize();
	}

	public MenuLayout GetMenu()
	{
		return m_Menu;
	}

	public Sony.NP.ErrorCode ErrorHandler(Sony.NP.ErrorCode errorCode = Sony.NP.ErrorCode.NP_ERR_FAILED)
	{
		Sony.NP.ResultCode result = new Sony.NP.ResultCode();
		Sony.NP.Commerce.GetLastError(out result);
		if (errorCode != Sony.NP.ErrorCode.NP_OK)
		{
			if (result.lastError != Sony.NP.ErrorCode.NP_OK && errorCode != Sony.NP.ErrorCode.NP_ERR_COMMERCE_SESSION_EXPIRED)
			{
				OnScreenLog.Add("Error: " + result.className + ": " + result.lastError + ", sce error 0x" + result.lastErrorSCE.ToString("X8"));
			}
			return result.lastError;
		}

		return errorCode;
	}

	public void Initialize()
	{
		m_Menu = new MenuLayout(this, 450, 34);

		Sony.NP.Commerce.OnSessionCreated += OnSessionCreated;
		Sony.NP.Commerce.OnSessionAborted += OnSomeEvent;
		Sony.NP.Commerce.OnGotCategoryInfo += OnGotCategoryInfo;
		Sony.NP.Commerce.OnGotProductList += OnGotProductList;
		Sony.NP.Commerce.OnGotProductInfo += OnGotProductInfo;
		Sony.NP.Commerce.OnCheckoutStarted += OnCheckoutStarted;
		Sony.NP.Commerce.OnCheckoutFinished += OnSomeEvent;

#if UNITY_PS4 || UNITY_PS3 // Note; these events are not available for PSP2
		Sony.NP.Commerce.OnProductBrowseStarted += OnSomeEvent;
		Sony.NP.Commerce.OnProductBrowseSuccess += OnSomeEvent;
		Sony.NP.Commerce.OnProductBrowseAborted += OnSomeEvent;
		Sony.NP.Commerce.OnProductBrowseFinished += OnSomeEvent;
		Sony.NP.Commerce.OnProductVoucherInputStarted += OnSomeEvent;
		Sony.NP.Commerce.OnProductVoucherInputFinished += OnSomeEvent;
#endif
	}

	public void OnEnter()
	{
		Sony.NP.Commerce.OnError += OnError;
		ErrorHandler(Sony.NP.Commerce.CreateSession());
		Sony.NP.Commerce.ShowStoreIcon(Sony.NP.Commerce.StoreIconPosition.Center);
	}

	public void OnExit()
	{
		Sony.NP.Commerce.OnError -= OnError;
		Sony.NP.Commerce.HideStoreIcon();
	}

	Sony.NP.ErrorCode CreateSessionThenDoSessionRequest(IRequest request)
	{
		Sony.NP.ErrorCode errCode = Sony.NP.Commerce.CreateSession();
		if (errCode == Sony.NP.ErrorCode.NP_OK)
		{
			// Store the request, it will be executed when session creation completes.
			m_PendingRequest = request;
		}
		return errCode;
	}

	Sony.NP.ErrorCode DoSessionRequest(IRequest request)
	{
		// Store the request in case we need to retry it after an session expired error.
		m_PendingRequest = request;

		// Try the request
		return m_PendingRequest.DoRequest();
	}

	void ClearPendingSessionRequest()
	{
		m_PendingRequest = null;
	}

	public void Process(MenuStack stack)
	{
		bool commerceReady = Sony.NP.User.IsSignedInPSN && !Sony.NP.Commerce.IsBusy();

		m_Menu.Update();

		if (!m_SessionCreated)
		{
			if (m_Menu.AddItem("Create Session", commerceReady))
			{
				ErrorHandler(Sony.NP.Commerce.CreateSession());
			}
		}
		
		if (m_Menu.AddItem("Root category Info", commerceReady && m_SessionCreated))
        {
            // Request info for a specified category, pass category ID or "" to get the root category.
            // A commerce session must be created before we can call RequestCategoryInfo().
            ErrorHandler(DoSessionRequest(new RequestCategoryInfo("")));
        }

        if (m_Menu.AddItem("Root product List", commerceReady && m_SessionCreated))
        {
            // Request the product list for a specified category, pass category ID or "" to get the root category.
            // A commerce session must be created before we can call RequestProductList().
            OnScreenLog.Add("Requesting  product list using ROOT category ID");
            ErrorHandler(DoSessionRequest(new RequestProductList("")));
        }

		if (m_Menu.AddItem("Category Info", commerceReady && m_SessionCreated))
		{
			// Request info for a specified category, pass category ID or "" to get the root category.
			// A commerce session must be created before we can call RequestCategoryInfo().
			OnScreenLog.Add("Requesting  Category info using category ID :" + m_TestCategoryID.ToString());
			ErrorHandler(DoSessionRequest(new RequestCategoryInfo("")));
		}
		
		if (m_Menu.AddItem("Product List", commerceReady && m_SessionCreated))
		{
			// Request the product list for a specified category, pass category ID or "" to get the root category.
			// A commerce session must be created before we can call RequestProductList().
			OnScreenLog.Add("Requesting  product list using category ID :" + m_TestCategoryID.ToString());
			ErrorHandler(DoSessionRequest(new RequestProductList(m_TestCategoryID)));
		}

		if (m_Menu.AddItem("Product Info", commerceReady && m_SessionCreated))
		{
			// Request detailed product info, pass a product ID.
			// A commerce session must be created before we can call RequestDetailedProductInfo().
			ErrorHandler(DoSessionRequest(new RequestDetailedProductInfo(m_TestProductID)));
		}

		if (m_Menu.AddItem("Checkout", commerceReady && m_SessionCreated))
		{
			// Open the Playstation store checkout dialog to purchase a specified list of products, pass an array of product Sku IDs.
			//
			// The commerce dialog requires a valid commerce session and because the commerce session might have expired on the server and
			// there is no way of knowing ahead of time if this has happened we should always create/recreate the session before calling
			// Checkout, this is handled by CreateSessionThenDoSessionRequest() which creates the session and schedules the checkout call
			// for when the session creation completes (in the OnSessionCreated function).
			ErrorHandler(CreateSessionThenDoSessionRequest(new RequestCheckout(m_TestProductSkuIDs)));
		}

		if (m_Menu.AddItem("Browse Product", commerceReady))
		{
			// Open the Playstation Store to a specified product, pass product ID.
			ErrorHandler(Sony.NP.Commerce.BrowseProduct(m_TestProductID));
		}

		if (m_Menu.AddItem("Redeem Voucher", commerceReady))
		{
			ErrorHandler(Sony.NP.Commerce.VoucherInput());
		}

		if (m_Menu.AddBackIndex("Back"))
		{
			stack.PopMenu();
		}
	}

	void OnSomeEvent(Sony.NP.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("Event: " + msg.type);
	}

	void OnCheckoutStarted(Sony.NP.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("Event: " + msg.type);

		// Clear the session request so it won't be processed again.
		ClearPendingSessionRequest();
	}

	void OnSessionCreated(Sony.NP.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("Commerce Session Created @ " + System.DateTime.Now.ToString("HH:mm:ss"));
		m_SessionCreated = true;

		// Is there is a commerce session request pending?
		if (m_PendingRequest != null)
		{
			OnScreenLog.Add("Executing pending request " + m_PendingRequest.ToString());
			m_PendingRequest.DoRequest();
		}
	}

	void OnGotCategoryInfo(Sony.NP.Messages.PluginMessage msg)
	{
		// Got the result so clear the session request so it won't be processed again.
		ClearPendingSessionRequest();

		OnScreenLog.Add("Got Category Info");
		Sony.NP.Commerce.CommerceCategoryInfo category = Sony.NP.Commerce.GetCategoryInfo();
		OnScreenLog.Add("Category Id: " + category.categoryId);
		OnScreenLog.Add("Category Name: " + category.categoryName);
		OnScreenLog.Add("Category num products: " + category.countOfProducts);
		OnScreenLog.Add("Category num sub categories: " + category.countOfSubCategories);

		for (int i = 0; i < category.countOfSubCategories; i++)
		{
			Sony.NP.Commerce.CommerceCategoryInfo subCategory = Sony.NP.Commerce.GetSubCategoryInfo(i);
			OnScreenLog.Add("SubCategory Id: " + subCategory.categoryId);
			OnScreenLog.Add("SubCategory Name: " + subCategory.categoryName);
			if (i == 0)
			{
				// Just for testing; request info for the 1st sub-category.
				ErrorHandler(Sony.NP.Commerce.RequestCategoryInfo(subCategory.categoryId));
			}
		}
	}

	void OnGotProductList(Sony.NP.Messages.PluginMessage msg)
	{
		// Got the result so clear the session request so it won't be processed again.
		ClearPendingSessionRequest();

		OnScreenLog.Add("Got Product List");
		Sony.NP.Commerce.CommerceProductInfo[] products = Sony.NP.Commerce.GetProductList();
		if (products.Length > 0)
		{
			foreach (Sony.NP.Commerce.CommerceProductInfo product in products)
			{
				OnScreenLog.Add("Product: " + product.productName + " - " + product.price);
			}
		}
		else
		{
			OnScreenLog.Add("No products found");
		}
	}

	void OnGotProductInfo(Sony.NP.Messages.PluginMessage msg)
	{
		// Got the result so clear the session request so it won't be processed again.
		ClearPendingSessionRequest();

		OnScreenLog.Add("Got Detailed Product Info");
		Sony.NP.Commerce.CommerceProductInfoDetailed product = Sony.NP.Commerce.GetDetailedProductInfo();
		OnScreenLog.Add("Product: " + product.productName + " - " + product.price);
		OnScreenLog.Add("Long desc: " + product.longDescription);
	}

	void OnError(Sony.NP.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("In-game commerce error...");
		Sony.NP.ResultCode result = new Sony.NP.ResultCode();
		Sony.NP.Commerce.GetLastError(out result);

		// If the error was caused by the commerce server session expiring then recreate the session
		// and retry the request that failed.
		if (result.lastError == Sony.NP.ErrorCode.NP_ERR_COMMERCE_SESSION_EXPIRED)
		{
			OnScreenLog.Add("Commerce server session expired");
			OnScreenLog.Add("Re-creating commerce session");

			// Recreate the session, the OnSessionCreated handler will do the request retry when the
			// session creation completes.
			ErrorHandler(Sony.NP.Commerce.CreateSession());
		}
		else
		{
			// An unrecoverable error so clear the current request and call the default error handler.
			ClearPendingSessionRequest();
			ErrorHandler();
		}
	}
}
