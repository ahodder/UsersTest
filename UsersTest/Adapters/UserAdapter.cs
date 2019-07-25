using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Views;
using Android.Widget;
using UsersTest.Models;

namespace UsersTest.Adapters {
  internal class UserAdapter : RecyclerView.Adapter {
    public Context Context { get; }
    public User this[int position] => _users[position];
      
    public override int ItemCount => _users.Count;
    
    private readonly List<User> _users;
    private readonly IOnUserInteraction _onUserInteraction;

    public UserAdapter(Context context, List<User> users, IOnUserInteraction onUserInteraction) {
      Context = context;
      _users = users;
      _onUserInteraction = onUserInteraction;
    }
      
    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
      if (!(holder is ViewHolder vh)) return;

      var user = _users[position];
      vh.userName.Text = $"[{user.Id}] {user.UserName}";
      vh.firstLastName.Text = $"{user.FirstName} {user.LastName}";
    }

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
      var view = LayoutInflater.From(parent.Context)
        .Inflate(Resource.Layout.list_item_user, parent, false);
      return new ViewHolder(this, view, _onUserInteraction);
    }

    public void DeleteItem(int position) {
      _onUserInteraction.UserDeleted(position, _users[position]);
      _users.RemoveAt(position);
      NotifyItemRemoved(position);
    }

    public void InsertItem(int position, User user) {
      _users.Insert(position, user);
      NotifyItemInserted(position);
    }
      
    public class ViewHolder : RecyclerView.ViewHolder, View.IOnClickListener {
      public TextView userName;
      public TextView firstLastName;

      private UserAdapter _adapter;
      private IOnUserInteraction _onUserInteraction;
      
      public ViewHolder(UserAdapter adapter, View itemView, IOnUserInteraction onUserInteraction) : base(itemView) {
        _adapter = adapter;
        itemView.SetOnClickListener(this);
        userName = itemView.FindViewById<TextView>(Resource.Id.user_name);
        firstLastName = itemView.FindViewById<TextView>(Resource.Id.first_last_name);
        _onUserInteraction = onUserInteraction;
      }

      public void OnClick(View v) {
        _onUserInteraction?.UserClicked(AdapterPosition, _adapter[AdapterPosition]);
      }
    }

    public class SwipeToDelete : ItemTouchHelper.SimpleCallback {
      private UserAdapter _adapter;
      private Drawable _icon;
      private ColorDrawable _background;
      
      public SwipeToDelete(UserAdapter adapter) : base(0, ItemTouchHelper.Left) {
        _adapter = adapter;
        _icon = ContextCompat.GetDrawable(_adapter.Context, Resource.Mipmap.ic_delete_black_24dp);
        _background = new ColorDrawable(Color.Red);
      }
      
      public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target) {
        return false;
      }

      public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction) {
        _adapter.DeleteItem(viewHolder.AdapterPosition);
      }

      public override void OnChildDraw(Canvas c, RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, float dX, float dY, int actionState, bool isCurrentlyActive) {
        base.OnChildDraw(c, recyclerView, viewHolder, dX, dY, actionState, isCurrentlyActive);
        var itemView = viewHolder.ItemView;
        var offset = 20;
        
        if (dX < 0) {
          _background.SetBounds(itemView.Right + ((int)dX - offset), itemView.Top, itemView.Right, itemView.Bottom);
          
          var margin = (itemView.Height - _icon.IntrinsicHeight) / 2;
          var top = itemView.Top + (itemView.Height - _icon.IntrinsicHeight) / 2;
          var bottom = top + _icon.IntrinsicHeight;
          var left = itemView.Right - margin - _icon.IntrinsicWidth;
          var right = itemView.Right - margin;
          _icon.SetBounds(left, top, right, bottom);
        } else {
          _background.SetBounds(0, 0, 0, 0);
        }
        
        _background.Draw(c);
        _icon.Draw(c);
      }

      public void Release() {
        _icon.Dispose();
      }
    }

    public interface IOnUserInteraction {
      void UserClicked(int position, User user);

      void UserDeleted(int position, User user);
    }
  }
}