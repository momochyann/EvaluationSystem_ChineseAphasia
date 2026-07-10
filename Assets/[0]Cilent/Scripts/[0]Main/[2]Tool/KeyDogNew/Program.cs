using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Runtime.InteropServices; // DllImport

namespace ViKeySample
{
    class Program
    {
        // 错误代码
        const long VIKEY_SUCCESS 						= 0x00000000; //成功
        const long VIKEY_ERROR_NO_VIKEY 				= 0x80000001; //没有找到ViKey加密锁
        const long VIKEY_ERROR_INVALID_PASSWORD 		= 0x80000002; //密码错误
        const long VIKEY_ERROR_NEED_FIND 				= 0x80000003; //请先查找加密锁
        const long VIKEY_ERROR_INVALID_INDEX 			= 0x80000004; //无效的句柄
        const long VIKEY_ERROR_INVALID_VALUE 			= 0x80000005; //数值错误
        const long VIKEY_ERROR_INVALID_KEY 				= 0x80000006; //秘钥无效
        const long VIKEY_ERROR_GET_VALUE 				= 0x80000007; //读取信息错误
        const long VIKEY_ERROR_SET_VALUE 				= 0x80000008; //设置信息错误
        const long VIKEY_ERROR_NO_CHANCE 				= 0x80000009; //没有机会
        const long VIKEY_ERROR_NO_TAUTHORITY 			= 0x8000000A; //权限不足
        const long VIKEY_ERROR_INVALID_ADDR_OR_SIZE 	= 0x8000000B; //地址或长度错误
        const long VIKEY_ERROR_RANDOM 					= 0x8000000C; //获取随机数错误
        const long VIKEY_ERROR_SEED 					= 0x8000000D; //获取种子错误
        const long VIKEY_ERROR_CONNECTION 				= 0x8000000E; //通信错误
        const long VIKEY_ERROR_CALCULATE 				= 0x8000000F; //算法或计算错误
        const long VIKEY_ERROR_MODULE 					= 0x80000010; //计数器错误
        const long VIKEY_ERROR_GENERATE_NEW_PASSWORD 	= 0x80000011; //产生密码错误
        const long VIKEY_ERROR_ENCRYPT_FAILED 			= 0x80000012; //加密数据错误
        const long VIKEY_ERROR_DECRYPT_FAILED 			= 0x80000013; //解密数据错误
        const long VIKEY_ERROR_ALREADY_LOCKED 			= 0x80000014; //ViKey加密锁已经被锁定
        const long VIKEY_ERROR_UNKNOWN_COMMAND 			= 0x80000015; //无效的命令
        const long VIKEY_ERROR_NO_SUPPORT 				= 0x80000016; //当前ViKey加密锁不支持此功能
        const long VIKEY_ERROR_CATCH 					= 0x80000017; //发生异常
        const long VIKEY_ERROR_GET_USBDATA 				= 0x80000018; //读取USB数据错误
        const long VIKEY_ERROR_SET_USBDATA 				= 0x80000019; //设置USB数据错误
        const long VIKEY_ERROR_TIME_MODULE 				= 0x8000001A; //硬件时钟模块异常
        const long VIKEY_ERROR_TIME_OUTAGE 				= 0x8000001B; //硬件时钟发生过断电，时钟可能不准确，需校准时钟
        const long VIKEY_ERROR_MAX_CONNECTION 			= 0x8000001C; //加密狗达到最大通信链接数
        const long VIKEY_ERROR_COMMUNICATION 			= 0x8000001D; //加密狗通信数据错误
        const long VIKEY_ERROR_TIME 					= 0x8000001E; //加密狗时钟错误
        const long VIKEY_ERROR_TIME_EXPIRE 				= 0x8000001F; //加密狗时钟限制已经到期
        const long VIKEY_ERROR_TIME_DISORDER 			= 0x80000020; //加密狗时钟失调
        const long VIKEY_ERROR_API_KEY 					= 0x80000021; //API密钥错误
        const long VIKEY_ERROR_UNKNOWN_ERROR 			= 0xFFFFFFFF; //未知错误

        //ViKey加密狗类型  VikeyGetType返回值代表的类型
        const uint ViKeyAPP = 0;   //实用型加密狗ViKeyAPP
        const uint ViKeySTD = 1;   //标准型加密狗ViKeySTD
        const uint ViKeyNET = 2;   //网络型加密狗ViKeyNET
        const uint ViKeyPRO = 3;   //专业型加密狗ViKeyPRO     
        const uint ViKeyWEB = 4;   //身份认证型加密狗ViKeyWEB
        const uint ViKeyTIME = 5;  //时间型加密狗ViKeyTIME

        // 函数引用声明
        [DllImport("ViKey")]
        public static extern uint VikeyGetLibraryVersion(Byte[] pVersion);	//获取接口库Dll版本号
        [DllImport("ViKey")]
        public static extern uint VikeySetUserSN(Byte[] pUserSN);
        [DllImport("ViKey")]
        public static extern uint VikeyFind(ref uint pdwCount);
        [DllImport("ViKey")]
        public static extern uint VikeyFindEx(ref uint pdwCount);
        [DllImport("ViKey")]
        public static extern uint VikeySetApiKey(ushort Index, Byte[] pApiKey);
        [DllImport("ViKey")]
        public static extern uint VikeyUninitialization();
        [DllImport("ViKey")]
        public static extern uint VikeyGetHID(ushort Index, ref uint pdwHID);
        [DllImport("ViKey")]
        public static extern uint VikeyGetType(ushort Index, ref uint pType);
        [DllImport("ViKey")]
        public static extern uint VikeyGetLevel(ushort Index, ref Byte pLevel);
        [DllImport("ViKey")]
        public static extern uint VikeyGetFirmVersion(ushort Index, Byte[] pVersion);	//获取加密狗硬件版本号
        [DllImport("ViKey")]
        public static extern uint VikeyGetUserDataSize(ushort Index, ref uint pBytes);
        [DllImport("ViKey")]
        public static extern uint VikeyGetAdminDataSize(ushort Index, ref uint pBytes);
        [DllImport("ViKey")]
        public static extern uint VikeySetPtroductName(ushort Index, Byte[] szName);
        [DllImport("ViKey")]
        public static extern uint VikeyGetPtroductName(ushort Index, Byte[] szName);
        [DllImport("ViKey")]
        public static extern uint VikeyUserLogin(ushort Index, Byte[] pUserPassword);
        [DllImport("ViKey")]
        public static extern uint VikeyAdminLogin(ushort Index, Byte[] pAdminPassword);
        [DllImport("ViKey")]
        public static extern uint VikeyLogoff(ushort Index);
        [DllImport("ViKey")]
        public static extern uint VikeySetUserPassWordAttempt(ushort Index, Byte cAttempt);
        [DllImport("ViKey")]
        public static extern uint VikeySetAdminPassWordAttempt(ushort Index, Byte cAttempt);
        [DllImport("ViKey")]
        public static extern uint VikeyGetUserPassWordAttempt(ushort Index, ref Byte pcCurrentAttempt, ref Byte pcMaxAttempt);
        [DllImport("ViKey")]
        public static extern uint VikeyGetAdminPassWordAttempt(ushort Index, ref Byte pcCurrentAttempt, ref Byte pcMaxAttempt);
        [DllImport("ViKey")]
        public static extern uint VikeyResetPassword(ushort Index, Byte[] pNewUserPassword, Byte[] pNewAdminPassword);
        [DllImport("ViKey")]
        public static extern uint VikeySetLED(ushort Index, Byte cLedState);
        [DllImport("ViKey")]
        public static extern uint VikeySetSoftIDString(ushort Index, Byte[] SoftIDString);
        [DllImport("ViKey")]
        public static extern uint VikeyGetSoftIDString(ushort Index, Byte[] SoftIDString);
        [DllImport("ViKey")]
        public static extern uint VikeyReadData(ushort Index, uint Address, uint Length, Byte[] pBuffer);
        [DllImport("ViKey")]
        public static extern uint VikeyWriteData(ushort Index, uint Address, uint Length, Byte[] pBuffer);
        [DllImport("ViKey")]
        public static extern uint ViKeyRandomBytes(ushort Index, uint Length, Byte[] pRandoms);
        [DllImport("ViKey")]
        public static extern uint ViKeySetModule(ushort Index, ushort ModelueIndex, ushort pValue, ushort pDecrease);
        [DllImport("ViKey")]
        public static extern uint ViKeyGetModule(ushort Index, ushort ModelueIndex, ref ushort pwValue);
        [DllImport("ViKey")]
        public static extern uint ViKeyCheckModule(ushort Index, ushort ModelueIndex, ref ushort pIsZero, ref ushort pCanDecrase);
        [DllImport("ViKey")]
        public static extern uint ViKeyDecraseModule(ushort Index, ushort ModelueIndex);
        [DllImport("ViKey")]
        public static extern uint Vikey3DesSetKey(ushort Index, Byte[] pKey, ushort KeyType);
        [DllImport("ViKey")]
        public static extern uint Vikey3DesEncrypt(ushort Index, ushort length, Byte[] pText,  Byte[] pResult);
        [DllImport("ViKey")]
        public static extern uint Vikey3DesDecrypt(ushort Index, ushort length, Byte[] pText,  Byte[] pResult);
        [DllImport("ViKey")]
        public static extern uint VikeyDesSetKey(ushort Index, Byte[] pKey);
        [DllImport("ViKey")]
        public static extern uint VikeyDesEncrypt(ushort Index, ushort length, Byte[] pText, Byte[] pResult, ref ushort OutLength);
        [DllImport("ViKey")]
        public static extern uint VikeyDesDecrypt(ushort Index, ushort length, Byte[] pText, Byte[] pResult, ref ushort OutLength);
        [DllImport("ViKey")]
        public static extern uint VikeyMD5(ushort Index, ushort length, Byte[] pText, Byte[] pResult);
        [DllImport("ViKey")]
        public static extern uint VikeySetMD5Key(ushort Index, Byte[] pMD5key);
        [DllImport("ViKey")]
        public static extern uint VikeyHmacMD5(ushort Index, ushort length, Byte[] pText, Byte[] pResult);
        [DllImport("ViKey")]
        public static extern uint VikeySHA1(ushort Index, ushort length, Byte[] pText, Byte[] pResult);
        [DllImport("ViKey")]
        public static extern uint VikeySetSHA1Key(ushort Index, Byte[] pSHA1key);
        [DllImport("ViKey")]
        public static extern uint VikeyHmacSHA1(ushort Index, ushort length, Byte[] pText, Byte[] pResult);
        [DllImport("ViKey")]
        public static extern uint VikeySM3(ushort Index, ushort length, Byte[] pText, Byte[] pResult);
        [DllImport("ViKey")]
        public static extern uint VikeyAesSetKey(ushort Index, Byte[] pKey);
        [DllImport("ViKey")]
        public static extern uint VikeyAesEncrypt(ushort Index, ushort length, Byte[] pText, Byte[] pResult, ref ushort OutLength);
        [DllImport("ViKey")]
        public static extern uint VikeyAesDecrypt(ushort Index, ushort length, Byte[] pText, Byte[] pResult, ref ushort OutLength);
        [DllImport("ViKey")]
        public static extern uint VikeySM1SetKey(ushort Index, Byte[] pKey);
        [DllImport("ViKey")]
        public static extern uint VikeySM1Encrypt(ushort Index, ushort length, Byte[] pText, Byte[] pResult, ref ushort OutLength);
        [DllImport("ViKey")]
        public static extern uint VikeySM1Decrypt(ushort Index, ushort length, Byte[] pText, Byte[] pResult, ref ushort OutLength);
        [DllImport("ViKey")]
        public static extern uint VikeySM4SetKey(ushort Index, Byte[] pKey);
        [DllImport("ViKey")]
        public static extern uint VikeySM4Encrypt(ushort Index, ushort length, Byte[] pText, Byte[] pResult, ref ushort OutLength);
        [DllImport("ViKey")]
        public static extern uint VikeySM4Decrypt(ushort Index, ushort length, Byte[] pText, Byte[] pResult, ref ushort OutLength);
        [DllImport("ViKey")]
        public static extern uint VikeySM2CreateKey(ushort Index, byte[] privateKey, byte[] publicKey);
        [DllImport("ViKey")]
        public static extern uint VikeySM2CalcPubKey(ushort Index, byte[] privateKey, byte[] publicKey);
        [DllImport("ViKey")]
        public static extern uint VikeySetSM2PrivateKey(ushort Index, byte[] privateKey);
        [DllImport("ViKey")]
        public static extern uint VikeySetSM2PublicKey(ushort Index, byte[] publicKey);
        [DllImport("ViKey")]
        public static extern uint VikeySM2Sign(ushort Index, byte[] userId, ushort userIdLength, byte[] signData, ushort signDataLength, byte[] signR, byte[] signS);
        [DllImport("ViKey")]
        public static extern uint VikeySM2Verify(ushort Index, byte[] userId, ushort userIdLength, byte[] data, ushort dataLength, byte[] signR, byte[] signS);
        [DllImport("ViKey")]
        public static extern uint VikeySM2Encrypt(ushort Index, Byte[] pData, ushort length, Byte[] pResult, ref ushort wResultLength);
        [DllImport("ViKey")]
        public static extern uint VikeySM2Decrypt(ushort Index, Byte[] pData, ushort length, Byte[] pResult, ref ushort wResultLength);
        [DllImport("ViKey")]
        public static extern uint VikeyGetTime(ushort Index, Byte[] pTime);
        [DllImport("ViKey")]
        public static extern uint VikeyGetValidTime(ushort Index, Byte[] pTime);
        [DllImport("ViKey")]
        public static extern uint VikeySetValidTime(ushort Index, Byte[] pTime);
        [DllImport("ViKey")]
        public static extern uint VikeyCheckValidTime(ushort Index, ref ushort pIsValid);

		public static void ViKeyTest()
        {
            UnityEngine.Debug.Log("********ViKey Untiy3D Sample********");

            uint HID, Count, ViKeyType = 0;
            Byte[] buffer = new Byte[256];
            uint retcode;
            ushort uIndex;
            ushort Addr, Length;
            ushort data1, data2, data3, data4;

            string str1 = "1234567890123456";
            string DefaultUserPassword = "11111111";
            string DefaultAdminPassword = "00000000";
            string strSoftIDString= "1234ABCD";
            string strApiKey = "2FF582392F72C0589326CB8FD647BF71B3EC18C39927158FB6D8AE7F241CACBF8AB346ED06614125BD412621832ECD3F585C63831E4AA9237E1F8762EB328546";
            string strUserSN = "9A178753CD7E072D7E1C82B0629F4820";

            HID = Count=0;
            data1 = data2 = data3 = data4 = 0;


			// 7.4.6版本(含)以后的加密狗必须在查找加密狗前调用VikeySetUserSN并输入正确的UserSN后面才能查找到所属用户的加密狗
			// 7.4.6版本以前的加密狗可以忽略此函数 不用调用此函数
			// UserSN在ViKey加密管理工具的->帮助中心页面->复制出来
			buffer = System.Text.Encoding.Default.GetBytes(strUserSN); 
			retcode = VikeySetUserSN(buffer);
			if (retcode != 0)
			{
				UnityEngine.Debug.Log("设置UserSN 失败 error code:"+ retcode);
				return;
			}
			
            //查找加密狗
            retcode =  VikeyFind(ref Count);
            if (retcode != 0)
            {
                UnityEngine.Debug.Log("查找ViKey加密狗错误 error code: "+ retcode);
                return;
            }
			UnityEngine.Debug.Log("查找ViKey加密狗个数:" + Count);

			uIndex = 0;
            //获取加密狗硬件序列号(HID)
			retcode = VikeyGetHID(uIndex, ref HID);
			if (retcode != 0)
			{
				UnityEngine.Debug.Log("获取硬件序列号错误 error code: " + retcode);
				return;
			}
			UnityEngine.Debug.Log("ViKey加密狗硬件序列号:" + HID);


            //获取加密狗型号
			retcode = VikeyGetType(uIndex, ref ViKeyType);
			if (retcode != 0)
			{
				UnityEngine.Debug.Log("获取ViKey加密狗型号 error code:"+ retcode);
				return;
			}
			UnityEngine.Debug.Log("获取ViKey加密狗型号:" + ViKeyType);

			//设置加密狗ApiKey 硬件版本为7.4.5(含)以后的加密狗必须要调用此接口才能登录
			//ApiKey 是由ViKey加密管理工具的->加密狗管理->密钥设置页面  通过Api密钥种子生成的
			buffer = System.Text.Encoding.Default.GetBytes(strApiKey); 
			retcode = VikeySetApiKey(uIndex, buffer);
			if (retcode != 0)
			{
				UnityEngine.Debug.Log("设置api密钥失败 error code:"+ retcode);
				return;
			}
			
            //用户登录加密狗
			buffer = System.Text.Encoding.Default.GetBytes(DefaultUserPassword); // convert unicode to asccii
			retcode = VikeyUserLogin(uIndex, buffer);
			if (retcode != 0)
			{
				UnityEngine.Debug.Log("用户登陆ViKey加密狗失败 error code: "+ retcode);
				return;
			}
			
			//管理员登陆加密狗
			buffer = System.Text.Encoding.Default.GetBytes(DefaultAdminPassword); // convert unicode to asccii
			retcode = VikeyAdminLogin(uIndex, buffer);
			if (retcode != 0)
			{
				UnityEngine.Debug.Log("管理员登陆ViKey加密狗失败 error code: " + retcode);
				return;
			}

                //ViKeyAPP不支持设置和读取设备名称
//                 string strPtroductName = "PtroductName";
//                 Byte[] szProductName = new Byte[32];
//                 szProductName = System.Text.Encoding.Unicode.GetBytes(strPtroductName);
//                 retcode =  VikeySetPtroductName(uIndex, szProductName);
//                 if (retcode != 0)
//                 {
                //                     UnityEngine.Debug.Log("设置设备名称失败 error code: "+ retcode);
//                 }
// 
//                 retcode =  VikeyGetPtroductName(uIndex, szProductName);
//                 if (retcode != 0)
//                 {
                //                     UnityEngine.Debug.Log("获取设备名称失败 error code: "+ retcode);
//                 }
//                 else
//                 {
//                     UnityEngine.Debug.Log("获取设备名称成功: " + System.Text.Encoding.Unicode.GetString(szProductName));
//                 }


			Addr = 0;			//写入地址
			Length = 16;		//写入长度
			buffer = System.Text.Encoding.Default.GetBytes(str1); // convert unicode to asccii
			
			//写入数据到加密狗
			retcode = VikeyWriteData(uIndex, Addr, Length, buffer);
			if (retcode != 0)
			{
				UnityEngine.Debug.Log("写入数据到加密狗失败 error code: "+retcode);
				return;
			}
			UnityEngine.Debug.Log("写入数据到加密狗成功:" + str1);



			//从加密狗中读取数据
            buffer = new Byte[str1.Length];
			retcode = VikeyReadData(uIndex, Addr, Length, buffer);
			if (retcode != 0)
			{
				UnityEngine.Debug.Log("从加密狗中读取数据失败 error code: "+ retcode);
				return;
			}
			str1 = System.Text.Encoding.ASCII.GetString(buffer);
			UnityEngine.Debug.Log("从加密狗中读取数据成功:" + str1);

			
			//设置加密狗的软件ID
			buffer = System.Text.Encoding.Default.GetBytes(strSoftIDString); // convert unicode to asccii
			retcode = VikeySetSoftIDString(uIndex, buffer);
			if (retcode != 0)
			{
				UnityEngine.Debug.Log("设置加密狗的软件ID error code: "+ retcode);
				return;
			}
			UnityEngine.Debug.Log("设置加密狗的软件ID成功: " + strSoftIDString);

			// read SoftID
			buffer = new Byte[strSoftIDString.Length];
			retcode = VikeyGetSoftIDString(uIndex, buffer);
			if (retcode != 0)
			{
				UnityEngine.Debug.Log("读取软件ID error code: "+ retcode);
				return;
			}
			strSoftIDString = System.Text.Encoding.ASCII.GetString(buffer);
			UnityEngine.Debug.Log("读取软件ID: " + strSoftIDString);

                //时钟狗支持获取时间
                //retcode = VikeyGetTime(uIndex, buffer);
                //if (retcode != 0)
                //{
                //    UnityEngine.Debug.Log("读取加密狗内部时间 error code: "+ retcode);
                //    return;
                //}
                //UnityEngine.Debug.Log("读取加密狗内部时间: " + buffer[0] + "\\" + buffer[1] + "\\" + buffer[2] + "\\ " +
                //buffer[3] + ":" + buffer[4] + ":" + buffer[5]);

               
                // Logoff ViKey
                retcode = VikeyLogoff(uIndex);
                if (retcode != 0)
                {
                    UnityEngine.Debug.Log("注销ViKey加密狗 error code: "+ retcode);
                    return;
                }
            //getch();
        }
    }
}
