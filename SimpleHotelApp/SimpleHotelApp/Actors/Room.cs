﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace SimpleHotelApp.Actors
{
    public class Room
    {
        private Int32 _id;
        private Int32 _number;
        private bool _busy;
        private String _guests;
        private Decimal _costPerDay;
        private Int32 _roomsCount;

        public Int32 ID
        {
            get { return _id; }
            set { _id = value; }
        }
        public Int32 Number
        {
            get { return _number; }
            set { _number = value; }
        }
        public bool Busy
        {
            get { return _busy; }
            set { _busy = value; }
        }
        
        public String Guests 
        {
            get {
                if (Busy)
                {
                    return _guests;
                }
                return String.Empty;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    Busy = true;
                else
                    Busy = false;
                _guests = value;
            }
        }

        public Decimal CostPerDay
        {
            get { return _costPerDay; }
            set { _costPerDay = value; }
        }

        public Int32 RoomsCount
        {
            get { return _roomsCount; }
            set { _roomsCount = value; }
        }

        public Room(Int32 id, Int32 number, Boolean busy, String guests, Decimal costPerDay, int roomsCount)
        {
            ID = id;
            Number = number;
            Busy = busy;
            AddGuests(guests);
            CostPerDay = costPerDay;
            RoomsCount = roomsCount;
        }

        public void AddGuests(String g)
        {
            try
            {
                if (g == null)
                    return;
                if (_guests == null)
                    _guests = String.Empty;
                var glist = JsonConvert.DeserializeObject<List<int>>(g);
                var a = JsonConvert.DeserializeObject<List<int>>(_guests);
                if (a == null)
                    a = new List<int>();
                a.AddRange(glist);
                Guests = JsonConvert.SerializeObject(a);
            }
            catch
            { }
        }

        public void AddGuest(List<Room> list, int index, String g)
        {
            try
            {
                if (g == null)
                    return;
                if (_guests == null)
                    _guests = String.Empty;

                int fResult = 0;
                var rResult = list[index];
                foreach (var room in list)
                {
                    List<int> a;
                    if(room.Guests == null)
                        a = new List<int>();
                    else
                        a = JsonConvert.DeserializeObject<List<int>>(room.Guests);
                    fResult = a == null ? 0 : a.Find(x => x == Convert.ToInt32(g));
                    if (fResult != 0)
                    {
                        return;
                    }
                }
                var rGuests = JsonConvert.DeserializeObject<List<int>>(rResult.Guests);
                if (rGuests == null)
                    rGuests = new List<int>();
                rGuests.Add(Convert.ToInt32(g));

                Guests = JsonConvert.SerializeObject(rGuests);
            }
            catch
            { }
        }

        public static void FullUpdateTable(SQLiteConnection connection, List<Room> list)
        {
            var clearComand = "DELETE FROM tblRooms;";

            var sqlCommand = new SQLiteCommand(clearComand, connection);
            sqlCommand.ExecuteNonQuery();

            foreach (var r in list)
            {
                Room.Create(connection, r);
            }
        }

        public static Guest Create(SQLiteConnection connection, Room room)
        {
            SQLiteCommand insertSQL = new SQLiteCommand(
                "INSERT INTO tblRooms " +
                "(Id, Number, Busy, GuestId, CostPerDay, RoomsCount) " +
                "VALUES (?,?,?,?,?,?);", connection
                );
            insertSQL.Parameters.Add(new SQLiteParameter("Id", room.ID));
            insertSQL.Parameters.Add(new SQLiteParameter("Number", room.Number));
            insertSQL.Parameters.Add(new SQLiteParameter("Busy", room.Busy));
            insertSQL.Parameters.Add(new SQLiteParameter("GuestId", room.Guests));
            insertSQL.Parameters.Add(new SQLiteParameter("CostPerDay", room.CostPerDay));
            insertSQL.Parameters.Add(new SQLiteParameter("RoomsCount", room.RoomsCount));
            try
            {
                insertSQL.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
            }
            return new Guest();
        }
    }
}