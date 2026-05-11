type PagedList<T, TCursor> = {
  items: T[]
  nextCursor: TCursor
}

type Activity = {
  id: string
  title: string
  date: Date
  description: string
  category: string
  isCancelled: boolean
  city: string
  venue: string
  longitude: number
  latitude: number
  attendees: Profile[]
  isGoing: boolean
  isHost: boolean
  hostId: string
  hostDisplayName:string
  hostImageUrl?:string
};

type LocationIQSuggestion = {
  place_id: string
  osm_id: string
  osm_type: string
  licence: string
  lat: string
  lon: string
  boundingbox: string[]
  class: string
  type: string
  display_name: string
  display_place: string
  display_address: string
  address: LocationIQAddress
};

type LocationIQAddress = {
  name: string
  state: string
  country: string
  country_code: string
  city?: string
  town?:string
  village?:string
  county?: string
  road?: string
  neighbourhood?: string
  suburb?: string
  postcode?: string
}

type Profile = {
  id: string
  displayName: string
  bio?: string
  imageUrl?: string
  followerCount?:number
  followingCount?:number
  following?:boolean
};

type User = {
  id:string
  email:string
  displayName:string
  imageUrl?:string
}

type Photo = {
  id:string,
  url:string
}

type ChatComment = {
  id:string
  body:string
  createDate:Date
  userId:string
  activityId:string
  imageUrl?:string
  displayName:string
}

type UserActivity = {
  id:string,
  date: Date,
  category: string,
  title: string
}