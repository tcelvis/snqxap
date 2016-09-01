using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace snqxap
{
    class Girl
    {
        public string name { get; set; }        //名字
        public int type { get; set; }           //类型： 0 - AR, 1 - SMG, 2 - HG, 3 - RF, 4 - MG

        public int hp_index { get; set; }       //生命
        public int damage_index { get; set; }   //伤害
        public int precision_index { get; set; }//命中
        public int firerate_index { get; set; } //射速
        public int dodge_index { get; set; }    //闪避
        public int potential { get; set; }      //成长

        public int belt { get; set; }           //弹链

        //public int aura_pos_mask { get; set; }  // 光环位置表：从左向右 从上向下。bit0起，bit4轮空（5号位）
        public string aura_pos_array { get; set; }  // 光环位置表：从左向右 从上向下。1号起，5号轮空
        //public int aura_type_mask { get; set; } // 光环类型表：按类型序号nth bit mask
        public string aura_type_array { get; set; }
        public double aura_damage { get; set; }
        public double aura_firerate { get; set; }
        public double aura_precision { get; set; }
        public double aura_dodge { get; set; }
        public double aura_crit { get; set; }
        public double aura_skill_prob { get; set; }

    }

    class Helper
    {
        public static Girl toGirl(Gun gun)
        {
            Girl girl = new Girl {
                name = gun.name,
                type = gun.what - 2,
                hp_index = (int) gun.ratiohp,
                damage_index = (int) gun.ratiopow,
                precision_index = (int) gun.ratiohit,
                firerate_index = (int) gun.ratiorate,
                dodge_index = (int) gun.ratiododge,
                potential = (int) gun.eatratio,
                belt = gun.belt,

                aura_damage = gun.damageup,
                aura_firerate = gun.shotspeedup,
                aura_precision = gun.hitup,
                aura_dodge = gun.dodgeup,
                aura_crit = gun.critup,
                aura_skill_prob = gun.rateup,

            };

            girl.aura_pos_array = "";
            for (int i = 0; i < gun.number; i++)
            {
                int pos = (int) typeof(Gun).GetProperty("effect"+i).GetValue(gun, null);
                //girl.aura_pos_mask |= 0x1 << (pos - 1);
                girl.aura_pos_array += (Char)('0' + pos);
            }
            girl.aura_pos_array = new string(girl.aura_pos_array.OrderBy(c => c).ToArray());

            switch (gun.to)
            {
                case 0:
                    girl.aura_type_array = "";
                    break;
                case 1:
                    girl.aura_type_array = "01234"; // 全部5种
                    break;
                case 2:
                    girl.aura_type_array = "0";     // AR only
                    break;
                case 3:
                    girl.aura_type_array = "1";     // SMG only
                    break;
                case 4:
                    girl.aura_type_array = "2";     // HG only
                    break;
                default:
                    throw new System.ArgumentException("Unexpected value of Gun.to");
            }

            return girl;
        }

        public static void exportGuns(Gun[] guns, string filename)
        {
            // 生成标题行
            PropertyInfo[] properties = typeof(Gun).GetProperties();

            System.IO.StreamWriter file = new System.IO.StreamWriter(filename);
            string line = "";
            foreach (PropertyInfo prop in properties)
            {
                line += prop.Name + ";";
            }
            line += "\n";
            file.Write(line);

            // 生成数据行
            foreach (Gun g in guns)
            {
                line = "";
                foreach (PropertyInfo prop in properties)
                {
                    line += prop.GetValue(g, null) + ";";
                }
                line += "\n";
                file.Write(line);
            }

            file.Close();
        }

        public static void exportGunsAsGirls(Gun[] guns, string filename)
        {
            // 生成标题行
            PropertyInfo[] properties = typeof(Girl).GetProperties();

            System.IO.StreamWriter file = new System.IO.StreamWriter(filename);
            string line = "";
            foreach (PropertyInfo prop in properties)
            {
                line += prop.Name + ";";
            }
            line += "\n";
            file.Write(line);

            // 生成数据行
            foreach (Gun g in guns)
            {
                Girl girl = toGirl(g);
                line = "";
                foreach (PropertyInfo prop in properties)
                {
                    line += prop.GetValue(girl, null) + ";";
                }
                line += "\n";
                file.Write(line);
            }

            file.Close();
        }
    }
}
