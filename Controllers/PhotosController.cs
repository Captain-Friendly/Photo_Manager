using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UsersManager.Models;

namespace UsersManager.Controllers
{
    [UserAccess]
    public class PhotosController : Controller
    {
        //photos
        // GET: Photos
        UsersDBEntities DB = new UsersDBEntities();
        #region Partial Refresh
        public void RenewPhotosSerialNumber()
        {
            //HttpRuntime.Cache est la cache du server
            HttpRuntime.Cache["photosSerialNumber"] = Guid.NewGuid().ToString();
        }

        public string GetPhotosSerialNumber()
        {
            if (HttpRuntime.Cache["photosSerialNumber"] == null)
            {
                RenewPhotosSerialNumber();
            }
            return (string)HttpRuntime.Cache["photosSerialNumber"];
        }

        public void SetLocalPhotosSerialNumber()
        {
            Session["photosSerialNumber"] = GetPhotosSerialNumber();
        }

        public bool IsPhotosUpToDate()
        {
            // session c'est la cache du user et http Runtime c'est la cache du joueur
            return ((string)Session["photosSerialNumber"] == (string)HttpRuntime.Cache["photosSerialNumber"]);
        }


        #endregion



        [UserAccess]
        public ActionResult Index()
        {
            SetLocalPhotosSerialNumber();
            Session["RatingFieldToSort"] = "dates";
            Session["RatingFieldSortDir"] = false;
            Session["SearchKeywords"] = "";
            return View();
        }

        public PartialViewResult GetPhotos(bool forceRefresh = false)
        {
            if (forceRefresh || !IsPhotosUpToDate())
            {
                SetLocalPhotosSerialNumber();
                ViewBag.FriendShipState = DB.FriendShipsStatus(OnlineUsers.CurrentUserId);
                ViewBag.CurrentUserId = OnlineUsers.CurrentUserId;
                if ((string)Session["SearchKeywords"] != "")
                {
                    var photos = UsersDBDAL.SearchPhotosByKeywords(DB.Photos.ToList(), (string)Session["SearchKeywords"]);
                    return PartialView(photos);
                }
                else
                {
                    var photos = DB.Photos;
                    return PartialView(photos);
                }
            }
            return null;
        }

        public ActionResult SetSearchKeywords(string keywords)
        {
            Session["SearchKeywords"] = keywords;
            return null;
        }

        [UserAccess]
        public ActionResult Create()
        {
            ViewBag.PhotoVisibilities = SelectListItemConverter<PhotoVisibility>.Convert(DB.PhotoVisibilities.ToList());
            return View(new Photo());
        }

        [HttpPost]
        public ActionResult Create(Photo photo)
        {
            if (ModelState.IsValid)
            {
                DB.Add_Photo(photo);
                RenewPhotosSerialNumber();
                return RedirectToAction("Index");
            }
            return View(photo);
        }

        [UserAccess]
        public ActionResult Details(int id)
        {
            Photo photo = DB.Photos.Find(id);
            Session["RatingFieldToSort"] = "dates";
            Session["RatingFieldSortDir"] = false;
            return View(photo);
        }



        public ActionResult GetPhotoDetails(int photoId, bool forceRefresh = false)
        {
            Photo photo = DB.Photos.Find(photoId);
            ViewBag.CurrentUserId = OnlineUsers.CurrentUserId;
            ViewBag.Visility = photo.PhotoVisibility.Name;
            if (photo != null && (forceRefresh || IsPhotosUpToDate() == false))
            {
                SetLocalPhotosSerialNumber();
                return View(photo);
            }
                
            return null;
        }

        [UserAccess]
        public ActionResult Edit(int id)
        {
            ViewBag.PhotoVisibilities = SelectListItemConverter<PhotoVisibility>.Convert(DB.PhotoVisibilities.ToList());
            Photo image = DB.Photos.Find(id);
            if (image != null)
                return View(image);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(Photo photo)
        {
            if (ModelState.IsValid)
            {
                DB.Update_Photo(photo);
                RenewPhotosSerialNumber();
                return RedirectToAction("Index");
            }
            return View(photo);
        }

        public ActionResult Delete(int id)
        {
            DB.Remove_Photo(id);
            RenewPhotosSerialNumber();
            return RedirectToAction("Index");
        }

        public ActionResult UpdateCurrentUserRating(int photoId,int rating, string comment)
        {
            PhotoRating photoRating = new PhotoRating();
            photoRating.PhotoId = photoId;
            photoRating.UserId = OnlineUsers.CurrentUserId;
            photoRating.Rating = rating;
            photoRating.CreationDate = DateTime.Now;
            photoRating.Comment = comment;
            DB.AddPhotoRating(photoRating);
            DB.Update_Photo_Ratings();
            RenewPhotosSerialNumber();
            return null;
        }

        public ActionResult SortRatingsBy(string fieldToSort)
        {
            if (fieldToSort == (string)Session["RatingFieldToSort"])
            {
                if ((bool)Session["RatingFieldSortDir"] == true)
                {
                    Session["RatingFieldSortDir"] = false;
                }
                else
                {
                    Session["RatingFieldSortDir"] = true;
                }
            }
            else
            {
                Session["RatingFieldToSort"] = fieldToSort;
                Session["RatingFieldSortDir"] = true;
            }
            return null;
        }

    }
}