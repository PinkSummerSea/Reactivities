import { Box, Button, Paper, Typography } from "@mui/material";
import { useActivities } from "../../../hooks/useActivities";
import { useNavigate, useParams } from "react-router";
import {useForm} from 'react-hook-form';
import { useEffect } from "react";
import { activitySchema, type ActivitySchema } from "../../../lib/schemas/activitySchema";
import {zodResolver} from '@hookform/resolvers/zod';
import TextInput from "../../../app/shared/components/TextInput";
import SelectInput from "../../../app/shared/components/SelectInput";
import { categoryOptions } from "./categoryOptions";
import DateTimeInput from "../../../app/shared/components/DateTimeInput";
import LocationInput from "../../../app/shared/components/LocationInput";

export default function ActivityForm() {
  const {id} = useParams();
  const { reset, control, handleSubmit } = useForm({
    mode: "onTouched",
    resolver: zodResolver(activitySchema),
    defaultValues: {
      title: "",
      description: "",
      category: "",
      date: new Date(),
      location: {
        venue: "",
        city: "",
        latitude: 0,
        longitude: 0,
      },
    },
  });
  const { updateActivity, createActivity, activity, isLoadingActivity } = useActivities(id);
  const navigate = useNavigate();
  const onSubmit = async (data:ActivitySchema) => {
    const {location, ...rest} = data;
    const flattenedData = {...rest, ...location};
    console.log(flattenedData)
    try {
      if (activity) {
        updateActivity.mutate(
          { ...activity, ...flattenedData } as Activity,
          {
            onSuccess: () => navigate(`/activities/${activity.id}`),
          },
        );
      } else {
        createActivity.mutate(flattenedData as Activity, {
          onSuccess: (id) => {
            navigate(`/activities/${id}`);
          },
        });
      }
    } catch (error) {
      console.log(error)
    }
    

  };

  useEffect(() => {
    if(activity) {
      reset({
        ...activity,
        location: {
          city:activity.city,
          venue:activity.venue,
          latitude:activity.latitude,
          longitude:activity.longitude
        }
      });
    }
  },[activity, reset])

  if(isLoadingActivity) return <Typography>loading</Typography>

  return (
    <Paper sx={{ borderRadius: 3, padding: 3 }}>
      <Typography variant="h5" gutterBottom color="primary">
        {id ? "Edit activity" : "Create activity"}
      </Typography>
      <Box
        component="form"
        onSubmit={handleSubmit(onSubmit)}
        display="flex"
        flexDirection="column"
        gap={3}
      >
        <TextInput label="Title" control={control} name="title" />
        <TextInput
          label="Description"
          control={control}
          name="description"
          multiline
          rows={3}
        />
        <Box display='flex' gap={3}>
          <SelectInput
            label="Category"
            control={control}
            items={categoryOptions}
            name="category"
          />
          <DateTimeInput label="Date" control={control} name="date" />
        </Box>
        
        <LocationInput
          label="Enter the Location"
          control={control}
          name="location"
        />

        <Box display="flex" justifyContent="end" gap={3}>
          <Button color="inherit">Cancel</Button>
          <Button
            type="submit"
            color="success"
            variant="contained"
            disabled={updateActivity.isPending || createActivity.isPending}
          >
            Submit
          </Button>
        </Box>
      </Box>
    </Paper>
  );
}
