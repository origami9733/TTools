//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Input;
//using TTools.Domain;
//using TTools.Views;
//using Microsoft.Practices.Prism.Commands;
//using System.ComponentModel;

//using TTools.EF; //DB関連
//using MaterialDesignThemes.Wpf;
//using System.Windows.Controls.Primitives;
//using System.Threading;


//namespace TTools.ViewModels
//{
//    public class JyuchuViewModel
//    {
//        public TechnoDB Context = new TechnoDB();


//        //モデルのメソッドをコマンド化する例
//        //private ICommand _addQuantityCommand;
//        //public ICommand AddQuantityCommand
//        //{
//        //    get
//        //    {
//        //        if(_addQuantityCommand == null)
//        //        {
//        //            _addQuantityCommand = new DelegateCommand(ExecuteAddQuantityCommand);
//        //        }
//        //        return _addQuantityCommand;
//        //    }
//        //}

//        //private void ExecuteAddQuantityCommand()
//        //{
//        //    if (CurrentRowItem == null) return;
//        //    CurrentRowItem.AddQuantity();
//        //}


//        //private BindingList<TTools.DataBase.Jyuchu> _bl1;
//        //public BindingList<TTools.DataBase.Jyuchu> Bl1 { get {return _bl1; } set { if (_bl1 == null) { _bl1 = value; }; } }

//        //public JyuchuViewModel()
//        //{
//        //    var aasds = from NAME in Context.Jyuchus select NAME;
//        //    Bl1 = new BindingList<TTools.DataBase.Jyuchu>(aasds.ToList());
//        //}


//        /// <summary>
//        ///     Observable化の例
//        ///     ToListメソッドでコンテキストにデータをロードした後、
//        ///     Localでオブジェクト参照渡しする
//        /// </summary>
//        //private ObservableCollection<TTools.DataBase.Jyuchu> _db;
//        //public ObservableCollection<TTools.DataBase.Jyuchu> DB
//        //{
//        //    get
//        //    {
//        //        if(_db == null)
//        //        {
//        //            Context.Set<TTools.DataBase.Jyuchu>().ToList();
//        //            _db = Context.Set<TTools.DataBase.Jyuchu>().Local;
//        //        }
//        //        return _db;
//        //    }
//        //}

        
//        ////SelectedItemのバインド先
//        //private TTools.DataBase.Jyuchu _currentRowItem;
//        //public TTools.DataBase.Jyuchu CurrentRowItem
//        //{
//        //    get { return _currentRowItem; }
//        //    set { _currentRowItem = value; }
//        //}
//    }
    
//}

