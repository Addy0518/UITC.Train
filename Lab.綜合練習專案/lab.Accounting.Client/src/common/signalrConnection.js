import * as signalR from '@microsoft/signalr'
import { useAuthStore } from '@/stores/authStore'

let connection = null

export const startConnection = async () => {
  const authStore = useAuthStore()
  const userId = authStore.userId

  connection = new signalR.HubConnectionBuilder()
    .withUrl(`/shopping-api/chatHub?userId=${userId}`, {
      // 把 JWT token 帶進去，後端才能驗證
      accessTokenFactory: () => authStore.token
    })
    .withAutomaticReconnect()
    .build()

  await connection.start()
  console.log('SignalR 連線成功')

  return connection
}

export const getConnection = () => connection

export const stopConnection = async () => {
  if (connection) {
    await connection.stop()
    connection = null
  }
}
